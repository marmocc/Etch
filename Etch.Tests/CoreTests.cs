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
    public void Intrinsics_InjectExtract_ShouldBeCorrect()
    {
        ulong storage = 0;
        ulong value = 0x0ABC;
        ulong mask = 0x0FFF;

        storage = Intrinsics.Inject(storage, value, mask);
        ulong read = Intrinsics.Extract(storage, mask);
        Assert.Equal(value, read);
    }

    [Fact]
    public void Intrinsics_Equals_ShouldBeCorrect()
    {
        RGBA c1 = new(8, 24, 84);
        RGBA c2 = new(1, 67, 244);
        Assert.False(Intrinsics.Equals<RGBA, uint>(c1, c2));
        RGBA c3 = new(1, 67, 244);
        Assert.True(Intrinsics.Equals<RGBA, uint>(c2, c3));
    }

    [Fact]
    public void Color_Basic_ShouldBeCorrect()
    {
        Assert.Equal(3, Unsafe.SizeOf<RGB>());
        Assert.Equal(4, Unsafe.SizeOf<RGBA>());
        Assert.Equal(Color.Black, default);
        Assert.Equal(255, Color.White.Luminance);
        Assert.Equal(0, Color.Black.Luminance);
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
        Assert.Equal(Glyph.Empty, default(Etchel).Glyph);
        Assert.Equal(Color.Black, default(Etchel).Foreground);
        Assert.Equal(Color.Black, default(Etchel).Background);
        Assert.Equal(Etchel.Empty, default);
    }

    [Fact]
    public void Etchel_Solid_ShouldBeCorrect()
    {
        Assert.Equal(Glyph.Empty, Etchel.Solid(Color.Green).Glyph);
        Assert.Equal(Color.Black, Etchel.Solid(Color.Green).Foreground);
        Assert.Equal(Color.Green, Etchel.Solid(Color.Green).Background);
    }
}
