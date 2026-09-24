using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Color(byte r, byte g, byte b, byte a = 255) : IEquatable<Color>
{
    public readonly byte R = r, G = g, B = b, A = a; // Fits into a uint

    public readonly byte Density
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            ReadOnlySpan<byte> ramp = " .:-=+*#%@"u8;
            int index = (8 * R + 26 * G + 3 * B) >> 10;
            if ((uint)index >= 10) index = 9;
            return ramp[index];
        }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(Color other) =>
        Unsafe.As<Color, uint>(ref Unsafe.AsRef(in this)) ==
        Unsafe.As<Color, uint>(ref Unsafe.AsRef(in other));
    public override bool Equals(object? obj) =>
        obj is Color color && Equals(color);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
    public override int GetHashCode() => (int)Unsafe.As<Color, uint>(ref Unsafe.AsRef(in this));
    public override string ToString() => $"({R},{G},{B},{A})";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color Blend(Color destination, Color source)
    {
        if (source.A == 255) return source;
        if (source.A == 0) return destination;

        int sA = source.A;
        int dA = 255 - sA;

        // High-speed integer approximation of standard alpha blending
        byte r = (byte)((source.R * sA + destination.R * dA) / 255);
        byte g = (byte)((source.G * sA + destination.G * dA) / 255);
        byte b = (byte)((source.B * sA + destination.B * dA) / 255);

        return new Color(r, g, b, 255);
    }
}
