namespace Etch.Common;

public readonly struct RGBA : IEquatable<RGBA>
{
    private static class Layout
    {
        public static readonly Field<uint> RGBA = new(0, 32);
        public static readonly Field<uint> R = new(0, 8);
        public static readonly Field<uint> G = new(8, 8);
        public static readonly Field<uint> B = new(16, 8);
        public static readonly Field<uint> A = new(24, 8);
    }

    public readonly uint Packed;

    public byte R => (byte)Layout.R.Read(Packed);
    public byte G => (byte)Layout.G.Read(Packed);
    public byte B => (byte)Layout.B.Read(Packed);
    public byte A => (byte)Layout.A.Read(Packed);

    public RGB RGB => new(R, G, B);
    public byte Luminance => RGB.Luminance;

    public RGBA(uint rgba)
    {
        Packed = Layout.RGBA.Read(rgba);
    }

    public RGBA(byte r, byte g, byte b, byte a)
    {
        Packed = Layout.R.Write(Packed, r)
               | Layout.G.Write(Packed, g)
               | Layout.B.Write(Packed, b)
               | Layout.A.Write(Packed, a);
    }

    public RGBA WithRed(byte red) => new(Layout.R.Write(Packed, red));
    public RGBA WithGreen(byte green) => new(Layout.G.Write(Packed, green));
    public RGBA WithBlue(byte blue) => new(Layout.B.Write(Packed, blue));
    public RGBA WithAlpha(byte alpha) => new(Layout.A.Write(Packed, alpha));

    public bool Equals(RGBA other) => Packed == other.Packed;
    public override bool Equals(object? obj) => obj is RGBA color && Equals(color);
    public static bool operator ==(RGBA left, RGBA right) => left.Equals(right);
    public static bool operator !=(RGBA left, RGBA right) => !(left == right);
    public override int GetHashCode() => Packed.GetHashCode();
    public override string ToString() => $"({R},{G},{B},{A})";
}