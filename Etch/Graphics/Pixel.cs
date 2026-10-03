using Etch.Common;

namespace Etch.Graphics;

public readonly struct Pixel(Color color) : IEquatable<Pixel>
{
    public static ReadOnlySpan<byte> Ramp => " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;

    public readonly Color Color = color;
    public int Luminance => (306 * Color.R + 601 * Color.G + 117 * Color.B) >> 10;
    public byte Glyph => Ramp[Luminance * Ramp.Length >> 8];

    public bool Equals(Pixel other) => Color == other.Color;
    public override bool Equals(object? obj) => obj is Pixel pixel && Equals(pixel);
    public static bool operator ==(Pixel left, Pixel right) => left.Equals(right);
    public static bool operator !=(Pixel left, Pixel right) => !(left == right);
    public override int GetHashCode() => Color.GetHashCode();
}
