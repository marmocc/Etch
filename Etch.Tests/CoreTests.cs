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
        Assert.Equal(7, Unsafe.SizeOf<Etchel>());
        Assert.Equal(Glyph.Empty, default(Etchel).Glyph);
        Assert.Equal(RGB.Black, default(Etchel).Foreground);
        Assert.Equal(RGB.Black, default(Etchel).Background);
        Assert.Equal(Etchel.Empty, default);
    }

    [Fact]
    public void Etchel_Packing_ShouldBeCorrect()
    {
        Etchel e = new(Glyph.From('#'), new RGB(1, 2, 3), new RGB(4, 5, 6));
        Assert.Equal(Glyph.From('#'), e.Glyph);
        Assert.Equal(new RGB(1, 2, 3), e.Foreground);
        Assert.Equal(new RGB(4, 5, 6), e.Background);
    }

    [Fact]
    public void Etchel_Solid_ShouldBeCorrect()
    {
        Assert.Equal(Glyph.Empty, Etchel.Solid(RGB.Green).Glyph);
        Assert.Equal(RGB.Black, Etchel.Solid(RGB.Green).Foreground);
        Assert.Equal(RGB.Green, Etchel.Solid(RGB.Green).Background);
    }
}
