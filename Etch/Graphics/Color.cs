using System.Drawing;
using System.Runtime.InteropServices;

namespace Etch.Graphics;

[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly struct Color(byte r, byte g, byte b, byte a = 255) : IEquatable<Color>
{
    [FieldOffset(0)] public readonly byte R = r;
    [FieldOffset(1)] public readonly byte G = g;
    [FieldOffset(2)] public readonly byte B = b;
    [FieldOffset(3)] public readonly byte A = a;

    // uint representation of the whole Color struct.
    [FieldOffset(0)] public readonly uint RGBA;

    public readonly byte Density
    {
        get
        {
            ReadOnlySpan<byte> ramp = " .:-=+*#%@"u8;
            int density = (8 * R + 26 * G + 3 * B) >> 10;
            if ((uint)density >= 10) density = 9;
            return ramp[density];
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

    public bool Equals(Color other) => this.RGBA == other.RGBA;
    public override bool Equals(object? obj) => obj is Color color && Equals(color);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
    public override int GetHashCode() => (int)this.RGBA;
    public override string ToString() => $"({R},{G},{B},{A})";

    public static void Blend(ref Color destination, Color source)
    {
        if (source.A == 0) return;
        if (source.A == 255) { destination = source; return; }

        int sA = source.A;
        int dA = 255 - sA;

        // High-speed integer approximation of standard alpha blending
        byte r = (byte)((source.R * sA + destination.R * dA) / 255);
        byte g = (byte)((source.G * sA + destination.G * dA) / 255);
        byte b = (byte)((source.B * sA + destination.B * dA) / 255);
        destination = new Color(r, g, b, 255);
    }

    public static void Blend(Span<Color> destination, Color source)
    {
        if (source.A == 0) return;
        if (source.A == 255) { destination.Fill(source); return; }

        for (int i = 0; i < destination.Length; i++)
            Color.Blend(ref destination[i], source);
    }
}