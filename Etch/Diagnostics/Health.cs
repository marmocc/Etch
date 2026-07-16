using System.Runtime.CompilerServices;
using Etch.Common;

namespace Etch.Diagnostics;

public static class Health
{
    private static void Assert(bool condition, [CallerArgumentExpression(nameof(condition))] string? expression = null)
    {
        if (!condition) throw new InvalidOperationException($"Check failed: {expression}");
    }

    private static void Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
    }

    public static void Check()
    {
        Fields();
        Colors();
        Glyphs();
        Etchels();
    }

    private static void Fields()
    {
        Assert(new Field<uint>(0, 32).Mask == uint.MaxValue);
        Assert(new Field<ulong>(0, 64).Mask == ulong.MaxValue);
        Assert(new Field<ulong>(40, 24).Read(ulong.MaxValue) == 0xFFFFFF);
        Assert(new Field<uint>(8, 8).Write(0xFFFFFFFF, 0) == 0xFFFF00FF);
        Assert(new Field<uint>(8, 8).Write(0, 0xAB) == 0x0000AB00);
        Assert(new Field<uint>(0, 32).Read(0xDEADBEEF) == 0xDEADBEEF);

        Throws<ArgumentOutOfRangeException>(() => new Field<uint>(0, 0));
        Throws<ArgumentOutOfRangeException>(() => new Field<uint>(-1, 8));
        Throws<ArgumentOutOfRangeException>(() => new Field<uint>(30, 8));
    }

    private static void Colors()
    {
        Assert(Unsafe.SizeOf<RGB>() == 4);
        Assert(Unsafe.SizeOf<RGBA>() == 4);

        Assert(default(RGB) == RGB.Black);
        Assert(new RGB(0xFFFFFFFF).Packed == 0x00FFFFFF);
        Assert(new RGB(1, 2, 3).Packed == 0x030201);
        Assert(new RGB(1, 2, 3).R == 1 && new RGB(1, 2, 3).G == 2 && new RGB(1, 2, 3).B == 3);
        Assert(new RGB(1, 2, 3).WithRed(9) == new RGB(9, 2, 3));
        Assert(RGB.White.Luminance == 255 && RGB.Black.Luminance == 0);

        Assert(default(RGBA).Packed == 0);
        Assert(new RGBA(1, 2, 3, 4).Packed == 0x04030201);
        Assert(new RGBA(1, 2, 3, 4).A == 4);
        Assert(new RGBA(1, 2, 3, 4).WithAlpha(9).R == 1);
        Assert(new RGBA(1, 2, 3, 4).WithAlpha(9).A == 9);

        Assert(RGB.White.RGBA.A == 255);
        Assert(RGB.White.RGBA.RGB == RGB.White);
        Assert(new RGBA(10, 20, 30, 0).RGB == new RGB(10, 20, 30));
    }

    private static void Glyphs()
    {
        Assert(Unsafe.SizeOf<Glyph>() == 1);

        Assert(default(Glyph).IsEmpty && default(Glyph).IsBlank);
        Assert(new Glyph(32).IsBlank && !new Glyph(32).IsEmpty);
        Assert(new Glyph(27).Codepoint == (byte)' ');
        Assert(Glyph.From('A').Index == 65 && Glyph.From('A').Codepoint == (byte)'A');
        Assert(Glyph.From(' ').Index == 32 && Glyph.From('~').Index == 126);
        Assert(Glyph.From('A').ToString() == "(65, 'A')");

        Throws<ArgumentOutOfRangeException>(() => Glyph.From('\n'));
        Throws<ArgumentOutOfRangeException>(() => Glyph.From('\u007F'));
        Throws<ArgumentOutOfRangeException>(() => Glyph.From('\u0141'));
    }

    private static void Etchels()
    {
        Assert(Unsafe.SizeOf<Etchel>() == 8);

        Assert(default(Etchel).Packed == 0);
        Assert(default(Etchel).Glyph == Glyph.Empty);
        Assert(default(Etchel).Foreground == RGB.Black && default(Etchel).Background == RGB.Black);
        Assert(Etchel.Empty == default);

        Etchel e = new(Glyph.From('#'), new RGB(1, 2, 3), new RGB(4, 5, 6));
        Assert(e.Packed == 0x0006050403020123UL);
        Assert(e.Glyph == Glyph.From('#'));
        Assert(e.Foreground == new RGB(1, 2, 3));
        Assert(e.Background == new RGB(4, 5, 6));
        Assert(e.ToString() == "'#' fg=(1,2,3) bg=(4,5,6)");

        Assert(e.WithForeground(RGB.Red).Glyph == e.Glyph);
        Assert(e.WithForeground(RGB.Red).Background == e.Background);
        Assert(e.WithForeground(RGB.Red).Foreground == RGB.Red);
        Assert(e.WithBackground(RGB.Blue).Glyph == e.Glyph);
        Assert(e.WithBackground(RGB.Blue).Foreground == e.Foreground);
        Assert(e.WithBackground(RGB.Blue).Background == RGB.Blue);
        Assert(e.WithGlyph(Glyph.From('A')).Foreground == e.Foreground);
        Assert(e.WithGlyph(Glyph.From('A')).Background == e.Background);
        Assert(e.WithGlyph(Glyph.From('A')).Glyph == Glyph.From('A'));

        Assert(Etchel.Solid(RGB.Green).Glyph == Glyph.Empty);
        Assert(Etchel.Solid(RGB.Green).Foreground == RGB.Black);
        Assert(Etchel.Solid(RGB.Green).Background == RGB.Green);

        Assert(Etchel.Solid(RGB.White).Packed == 0x00FFFFFF00000000UL);
        Assert((new Etchel(Glyph.From('~'), RGB.White, RGB.White).Packed >> 56) == 0);

        Assert(e == new Etchel(Glyph.From('#'), new RGB(1, 2, 3), new RGB(4, 5, 6)));
        Assert(e != e.WithForeground(RGB.Red));
    }
}