namespace Etch.Common;

public readonly struct RGB : IEquatable<RGB>
{
    private static class Layout
    {
        public static readonly Field<uint> RGB = new(0, 24);
        public static readonly Field<uint> R = new(0, 8);
        public static readonly Field<uint> G = new(8, 8);
        public static readonly Field<uint> B = new(16, 8);
    }

    public readonly uint Packed;

    public byte R => (byte)Layout.R.Read(Packed);
    public byte G => (byte)Layout.G.Read(Packed);
    public byte B => (byte)Layout.B.Read(Packed);

    public RGBA RGBA => new(R, G, B, 255);
    public byte Luminance => (byte)((306 * R + 601 * G + 117 * B) >> 10);

    public RGB(uint rgb)
    {
        Packed = Layout.RGB.Read(rgb);
    }

    public RGB(byte r, byte g, byte b)
    {
        Packed = Layout.R.Write(Packed, r)
               | Layout.G.Write(Packed, g)
               | Layout.B.Write(Packed, b);
    }

    public static RGB Black => new(0, 0, 0);
    public static RGB White => new(255, 255, 255);
    public static RGB Red => new(255, 0, 0);
    public static RGB Green => new(0, 255, 0);
    public static RGB Blue => new(0, 0, 255);

    public RGB WithRed(byte red) => new(Layout.R.Write(Packed, red));
    public RGB WithGreen(byte green) => new(Layout.G.Write(Packed, green));
    public RGB WithBlue(byte blue) => new(Layout.B.Write(Packed, blue));

    public bool Equals(RGB other) => Packed == other.Packed;
    public override bool Equals(object? obj) => obj is RGB color && Equals(color);
    public static bool operator ==(RGB left, RGB right) => left.Equals(right);
    public static bool operator !=(RGB left, RGB right) => !(left == right);
    public override int GetHashCode() => Packed.GetHashCode();
    public override string ToString() => $"({R},{G},{B})";
}
