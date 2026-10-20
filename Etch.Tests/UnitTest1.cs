using Xunit;
using Etch.Common;

namespace Etch.Tests;

public class CoreTests
{
    [Fact]
    public void Etchel_PackingAndUnpacking_ShouldBeConsistent()
    {
        // Arrange
        var glyph = new Glyph((byte)65); // 'A'
        var fg = new RGB(255, 0, 0);    // Red
        var bg = new RGB(0, 0, 255);    // Blue

        // Act
        var etchel = new Etchel(glyph, fg, bg);

        // Assert
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
}
