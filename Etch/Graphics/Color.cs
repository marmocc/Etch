using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Graphics;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Color : IEquatable<Color>
{
    public readonly byte R, G, B, A; // Amounts to uint
    public readonly byte Density;
    public static readonly byte[] Ramp = [32, 46, 58, 45, 61, 43, 42, 35, 37, 64]; // " .:-=+*#%@"

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r; G = g; B = b; A = a;
        float luminance = (0.2126f * R + 0.7152f * G + 0.0722f * B) / 255f;
        Density = Ramp[Math.Clamp((int)(luminance * Ramp.Length), 0, Ramp.Length - 1)];
    }

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

    public bool Equals(Color other) =>
        Unsafe.As<Color, uint>(ref Unsafe.AsRef(in this)) ==
        Unsafe.As<Color, uint>(ref Unsafe.AsRef(in other));
    public override bool Equals(object? obj) =>
        obj is Color color && Equals(color);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
    public override int GetHashCode() => HashCode.Combine(R, G, B, A);
    public override string ToString() => $"({R},{G},{B},{A})";

    public static Color Blend(Color destination, Color source)
    {
        if (source.A == 255) return source;
        if (source.A == 0) return destination;

        float alphaFactor = source.A / 255f;
        float invAlphaFactor = 1f - alphaFactor;

        byte r = (byte)(source.R * alphaFactor + destination.R * invAlphaFactor);
        byte g = (byte)(source.G * alphaFactor + destination.G * invAlphaFactor);
        byte b = (byte)(source.B * alphaFactor + destination.B * invAlphaFactor);

        return new(r, g, b, 255);
    }
}
