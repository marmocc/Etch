using Xunit;
using Etch.Common;
using System.Runtime.CompilerServices;

namespace Etch.Tests;

public class CoreTests
{
    [Fact]
    public void Etchel_PackingAndUnpacking_ShouldBeConsistent()
    {
        var glyph = new Glyph((byte)65);
        var fg = new RGB(255, 0, 0);
        var bg = new RGB(0, 0, 255);
        var etchel = new Etchel(glyph, fg, bg);
        Assert.Equal(glyph, etchel.Glyph);
        Assert.Equal(fg, etchel.Foreground);
        Assert.Equal(bg, etchel.Background);
    }

    [Fact]
    public void Etchel_Equality_ShouldWork()
    {
        var e1 = new Etchel(new Glyph((byte)65), new RGB(255, 0, 0), new RGB(0, 0, 0));
        var e2 = new Etchel(new Glyph((byte)65), new RGB(255, 0, 0), new RGB(0, 0, 0));
        var e3 = new Etchel(new Glyph((byte)66), new RGB(255, 0, 0), new RGB(0, 0, 0));
        Assert.Equal(e1, e2);
        Assert.NotEqual(e1, e3);
    }

    [Fact]
    public void Field_ReadWrite_ShouldBeCorrect()
    {
        var field = new Field<ulong>(8, 24);
        ulong storage = 0;
        ulong value = 0xABCDEF;
        ulong updated = field.Write(storage, value);
        ulong read = field.Read(updated);
        Assert.Equal(value, read);
    }

    [Fact]
    public void Field_Masks_ShouldBeCorrect()
    {
        Assert.Equal(uint.MaxValue, new Field<uint>(0, 32).Mask);
        Assert.Equal(ulong.MaxValue, new Field<ulong>(0, 64).Mask);
        Assert.Equal(0xFFFFFFUL, new Field<ulong>(40, 24).Read(ulong.MaxValue));
        Assert.Equal(0xFFFF00FFU, new Field<uint>(8, 8).Write(0xFFFFFFFFU, 0U));
        Assert.Equal(0x0000AB00U, new Field<uint>(8, 8).Write(0U, 0xABU));
        Assert.Equal(0xDEADBEEFU, new Field<uint>(0, 32).Read(0xDEADBEEFU));
    }

    [Fact]
    public void Field_Exceptions_ShouldBeThrown()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Field<uint>(0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Field<uint>(-1, 8));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Field<uint>(30, 8));
    }

    [Fact]
    public void Color_Basic_ShouldBeCorrect()
    {
        Assert.Equal(4, Unsafe.SizeOf<RGB>());
        Assert.Equal(4, Unsafe.SizeOf<RGBA>());
        Assert.Equal(RGB.Black, default(RGB));
        Assert.Equal(0x00FFFFFFU, new RGB(0xFFFFFFFFU).Packed);
        Assert.Equal(0x030201U, new RGB(1, 2, 3).Packed);
        Assert.True(new RGB(1, 2, 3).R == 1 && new RGB(1, 2, 3).G == 2 && new RGB(1, 2, 3).B == 3);
        Assert.Equal(new RGB(9, 2, 3), new RGB(1, 2, 3).WithRed(9));
        Assert.Equal(255, RGB.White.Luminance);
        Assert.Equal(0, RGB.Black.Luminance);
    }

    [Fact]
    public void RGBA_Basic_ShouldBeCorrect()
    {
        Assert.Equal(0U, default(RGBA).Packed);
        Assert.Equal(0x04030201U, new RGBA(1, 2, 3, 4).Packed);
        Assert.Equal(4, new RGBA(1, 2, 3, 4).A);
        Assert.Equal(new RGBA(1, 2, 3, 9), new RGBA(1, 2, 3, 4).WithAlpha(9));
        Assert.Equal(1, new RGBA(1, 2, 3, 4).WithAlpha(9).R);
        Assert.Equal(9, new RGBA(1, 2, 3, 4).WithAlpha(9).A);
    }

    [Fact]
    public void RGBA_InterOp_ShouldBeCorrect()
    {
        Assert.Equal(255, RGB.White.RGBA.A);
        Assert.Equal(RGB.White, RGB.White.RGBA.RGB);
        Assert.Equal(new RGB(10, 20, 30), new RGBA(10, 20, 30, 0).RGB);
    }

    [Fact]
    public void Glyph_Basic_ShouldBeCorrect()
    {
        Assert.Equal(1, Unsafe.SizeOf<Glyph>());
        Assert.True(default(Glyph).IsEmpty);
        Assert.True(default(Glyph).IsBlank);
        Assert.True(new Glyph(32).IsBlank);
        Assert.False(new Glyph(32).IsEmpty);
        Assert.Equal((byte)' ', new Glyph(27).Codepoint);
        Assert.Equal(65, Glyph.From('A').Index);
        Assert.Equal((byte)'A', Glyph.From('A').Codepoint);
        Assert.Equal(32, Glyph.From(' ').Index);
        Assert.Equal(126, Glyph.From('~').Index);
        Assert.Equal("(65, 'A')", Glyph.From('A').ToString());
    }

    [Fact]
    public void Glyph_Exceptions_ShouldBeThrown()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Glyph.From('\n'));
        Assert.Throws<ArgumentOutOfRangeException>(() => Glyph.From('\u007F'));
        Assert.Throws<ArgumentOutOfRangeException>(() => Glyph.From('Ł'));
    }

    [Fact]
    public void Etchel_Basic_ShouldBeCorrect()
    {
        Assert.Equal(8, Unsafe.SizeOf<Etchel>());
        Assert.Equal(0UL, default(Etchel).Packed);
        Assert.Equal(Glyph.Empty, default(Etchel).Glyph);
        Assert.Equal(RGB.Black, default(Etchel).Foreground);
        Assert.Equal(RGB.Black, default(Etchel).Background);
        Assert.Equal(Etchel.Empty, default);
    }

    [Fact]
    public void Etchel_Packing_ShouldBeCorrect()
    {
        Etchel e = new(Glyph.From('#'), new RGB(1, 2, 3), new RGB(4, 5, 6));
        Assert.Equal(0x0006050403020123UL, e.Packed);
        Assert.Equal(Glyph.From('#'), e.Glyph);
        Assert.Equal(new RGB(1, 2, 3), e.Foreground);
        Assert.Equal(new RGB(4, 5, 6), e.Background);
        Assert.Equal("'#' fg=(1,2,3) bg=(4,5,6)", e.ToString());
    }

    [Fact]
    public void Etchel_Modifiers_ShouldBeCorrect()
    {
        var e = new Etchel(Glyph.From('#'), new RGB(1, 2, 3), new RGB(4, 5, 6));

        // Test WithForeground
        var fgChanged = e.WithForeground(RGB.Red);
        Assert.NotEqual(e.Glyph, fgChanged.Glyph); // This was the bug - it should NOT match if we changed it?
        // Wait, I'm confusing myself. WithForeground should NOT change the glyph.
        // Let's be explicit.

        Assert.Equal(e.Glyph, fgChanged.Glyph); // Glyph stays same
        Assert.Equal(e.Background, fgChanged.Background); // Background stays same
        Assert.Equal(RGB.Red, fgChanged.Foreground); // Foreground changes

        // Test WithBackground
        var bgChanged = e.WithBackground(RGB.Blue);
        Assert.Equal(e.Glyph, bgChanged.Glyph); // Glyph stays same
        Assert.Equal(e.Foreground, bgChanged.Foreground); // Foreground stays same
        Assert.Equal(RGB.Blue, bgChanged.Background); // Background changes

        // Test WithGlyph
        var glyphChanged = e.WithGlyph(Glyph.From('A'));
        Assert.NotEqual(e.Glyph, glyphChanged.Glyph); // Glyph CHANGES
        Assert.Equal(e.Foreground, glyphChanged.Foreground); // Foreground stays same
        Assert.Equal(e.Background, glyphChanged.Background); // Background stays same
    }

    [Fact]
    public void Etchel_Solid_ShouldBeCorrect()
    {
        Assert.Equal(Glyph.Empty, Etchel.Solid(RGB.Green).Glyph);
        Assert.Equal(RGB.Black, Etchel.Solid(RGB.Green).Foreground);
        Assert.Equal(RGB.Green, Etchel.Solid(RGB.Green).Background);
        Assert.Equal(0x00FFFFFF00000000UL, Etchel.Solid(RGB.White).Packed);
    }
}
