namespace Etch.Graphics;

public readonly struct Color(byte r, byte g, byte b, byte a = 255) : IEquatable<Color>
{
    public const byte RShift = 0, GShift = 8, BShift = 16, AShift = 24;
    public readonly uint Packed = ((uint)r << RShift) | ((uint)g << GShift) | ((uint)b << BShift) | ((uint)a << AShift);

    public readonly byte R => (byte)(Packed >> RShift);
    public readonly byte G => (byte)(Packed >> GShift);
    public readonly byte B => (byte)(Packed >> BShift);
    public readonly byte A => (byte)(Packed >> AShift);

    public int Luminance => (306 * R + 601 * G + 117 * B) >> 10;

    public static Color Transparent => new(0, 0, 0, 0);
    public static Color White => new(255, 255, 255, 255);
    public static Color Black => new(0, 0, 0, 255);
    public static Color Red => new(255, 0, 0, 255);
    public static Color Green => new(0, 255, 0, 255);
    public static Color Blue => new(0, 0, 255, 255);

    public Color WithRed(byte red) => new(red, G, B, A);
    public Color WithGreen(byte green) => new(R, green, B, A);
    public Color WithBlue(byte blue) => new(R, G, blue, A);
    public Color WithAlpha(byte alpha) => new(R, G, B, alpha);

    public bool Equals(Color other) => Packed == other.Packed;
    public override bool Equals(object? obj) => obj is Color color && Equals(color);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
    public override int GetHashCode() => Packed.GetHashCode();
    public override string ToString() => $"({R},{G},{B},{A})";
}