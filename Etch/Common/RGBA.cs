namespace Etch.Common;

public readonly struct RGBA(byte r, byte g, byte b, byte a) : IEquatable<RGBA>
{
    public readonly byte R = r;
    public readonly byte G = g;
    public readonly byte B = b;
    public readonly byte A = a;

    public RGB RGB => new(R, G, B);
    public byte Luminance => RGB.Luminance;

    public bool Equals(RGBA other) => Intrinsics.Equality<RGBA, uint>(this, other);
    public override bool Equals(object? obj) => obj is RGBA color && Equals(color);
    public static bool operator ==(RGBA left, RGBA right) => left.Equals(right);
    public static bool operator !=(RGBA left, RGBA right) => !(left == right);
    public override int GetHashCode() => Intrinsics.Hashcode<RGBA, uint>(this);
    public override string ToString() => $"({R},{G},{B},{A})";
}