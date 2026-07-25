namespace Etch.Primitives;

public readonly record struct Float2(float X, float Y)
{
    public static Float2 operator +(Float2 a, Float2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Float2 operator -(Float2 a, Float2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Float2 operator *(Float2 v, float s) => new(v.X * s, v.Y * s);
}
