namespace Etch.Graphics;

public readonly struct Color(byte r, byte g, byte b, byte a = 255) : IEquatable<Color>
{
    public byte R { get; } = r;
    public byte G { get; } = g;
    public byte B { get; } = b;
    public byte A { get; } = a;

    public static Color White => new(255, 255, 255);
    public static Color Black => new(0, 0, 0);
    public static Color Red => new(255, 0, 0);
    public static Color Green => new(0, 255, 0);
    public static Color Blue => new(0, 0, 255);

    public bool Equals(Color other) =>
        R == other.R && G == other.G && B == other.B && A == other.A;
    public override bool Equals(object? obj) =>
        obj is Color color && Equals(color);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
    public override int GetHashCode() => HashCode.Combine(R, G, B, A);
}
