namespace Etch.Common;

public readonly record struct RGB(byte R, byte G, byte B)
{
    public byte Luminance => (byte)((306 * R + 601 * G + 117 * B) >> 10);
}
public readonly record struct RGBA(RGB RGB, byte A = 255)
{
    public RGBA(byte r, byte g, byte b, byte a = 255) : this(new(r, g, b), a) { }
}

public static class Color
{
    public static RGB Black => new(0, 0, 0);
    public static RGB White => new(255, 255, 255);
    public static RGB Red => new(255, 0, 0);
    public static RGB Green => new(0, 255, 0);
    public static RGB Blue => new(0, 0, 255);
}
