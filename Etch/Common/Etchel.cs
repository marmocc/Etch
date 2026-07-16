namespace Etch.Common;

public readonly struct Etchel : IEquatable<Etchel>
{
    private static class Layout
    {
        public static readonly Field<ulong> Etchel = new(0, 56);
        public static readonly Field<ulong> Glyph = new(0, 8);
        public static readonly Field<ulong> Foreground = new(8, 24);
        public static readonly Field<ulong> Background = new(32, 24);
    }

    public readonly ulong Packed;

    public Glyph Glyph => new((byte)Layout.Glyph.Read(Packed));
    public RGB Foreground => new((uint)Layout.Foreground.Read(Packed));
    public RGB Background => new((uint)Layout.Background.Read(Packed));

    public Etchel(ulong etchel)
    {
        Packed = Layout.Etchel.Read(etchel);
    }

    public Etchel(Glyph glyph, RGB foreground, RGB background)
    {
        Packed = Layout.Glyph.Write(Packed, glyph.Index)
               | Layout.Foreground.Write(Packed, foreground.Packed)
               | Layout.Background.Write(Packed, background.Packed);
    }

    public static Etchel Empty => default;
    public static Etchel Solid(RGB color) => new(Glyph.Empty, RGB.Black, color);
    public Etchel WithGlyph(Glyph glyph) => new(Layout.Glyph.Write(Packed, glyph.Index));
    public Etchel WithForeground(RGB foreground) => new(Layout.Foreground.Write(Packed, foreground.Packed));
    public Etchel WithBackground(RGB background) => new(Layout.Background.Write(Packed, background.Packed));

    public bool Equals(Etchel other) => Packed == other.Packed;
    public override bool Equals(object? obj) => obj is Etchel other && Equals(other);
    public static bool operator ==(Etchel left, Etchel right) => left.Equals(right);
    public static bool operator !=(Etchel left, Etchel right) => !(left == right);
    public override int GetHashCode() => Packed.GetHashCode();
    public override string ToString() => $"'{(char)Glyph.Codepoint}' fg={Foreground} bg={Background}";
}