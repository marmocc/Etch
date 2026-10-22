namespace Etch.Common;

public readonly record struct Etchel(Glyph Glyph, RGB Foreground, RGB Background)
{
    public static Etchel Empty => default;
    public static Etchel Solid(RGB color) => new(Glyph.Empty, RGB.Black, color);
}