namespace Etch.Common;

public readonly struct RGB(byte r, byte g, byte b) : IEquatable<RGB>
{
    public readonly byte R = r;
    public readonly byte G = g;
    public readonly byte B = b;
    private readonly byte _a = 255;

    public RGBA RGBA => new(R, G, B, 255);
    public byte Luminance => (byte)((306 * R + 601 * G + 117 * B) >> 10);

    public static RGB Black => new(0, 0, 0);
    public static RGB White => new(255, 255, 255);
    public static RGB Red => new(255, 0, 0);
    public static RGB Green => new(0, 255, 0);
    public static RGB Blue => new(0, 0, 255);

    public bool Equals(RGB other) => Intrinsics.Equality<RGB, uint>(this, other);
    public override bool Equals(object? obj) => obj is RGB color && Equals(color);
    public static bool operator ==(RGB left, RGB right) => left.Equals(right);
    public static bool operator !=(RGB left, RGB right) => !(left == right);
    public override int GetHashCode() => Intrinsics.Hashcode<RGB, uint>(this);
    public override string ToString() => $"({R},{G},{B})";
}
