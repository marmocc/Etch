namespace Etch.Primitives;

public readonly record struct Color(byte R, byte G, byte B, byte A = 255)
{
    public static Color FromRgb(byte r, byte g, byte b) => new(r, g, b);
}