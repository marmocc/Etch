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

    // Glyph representation of the Color's Luminance
    public readonly byte Glyph
    {
        get
        {
            int luminance = (306 * R + 601 * G + 117 * B) >> 10;
            int index = (luminance * 70) >> 8;
            ReadOnlySpan<byte> ramp = " .'`^\",_-~:;!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
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

    public bool Equals(Color other) => this.RGBA == other.RGBA;
    public override bool Equals(object? obj) => obj is Color color && Equals(color);
    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
    public override int GetHashCode() => this.RGBA.GetHashCode();
    public override string ToString() => $"({R},{G},{B},{A})";

    public static Color Blend(Color destination, Color source)
    {
        if (source.A == 0) return destination;
        if (source.A == 255) return source;

        int sA = source.A;
        int dA = 255 - sA;

        int rSum = source.R * sA + destination.R * dA;
        int gSum = source.G * sA + destination.G * dA;
        int bSum = source.B * sA + destination.B * dA;

        byte r = Utilities.Div255(rSum);
        byte g = Utilities.Div255(gSum);
        byte b = Utilities.Div255(bSum);

        return new Color(r, g, b, 255);
    }

    public static void Blend(Span<Color> destination, Color source)
    {
        if (source.A == 0) return;
        if (source.A == 255) { destination.Fill(source); return; }

        for (int i = 0; i < destination.Length; i++)
            destination[i] = Color.Blend(destination[i], source);
    }
}