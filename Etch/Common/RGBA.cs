namespace Etch.Common;

public readonly record struct RGBA(RGB RGB, byte A = 255)
{
    public RGBA(byte r, byte g, byte b, byte a = 255) : this(new(r, g, b), a) { }
}