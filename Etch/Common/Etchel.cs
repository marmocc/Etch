namespace Etch.Common;

public readonly struct Etchel(Glyph glyph, RGB foreground, RGB background) : IEquatable<Etchel>
{
    public readonly Glyph Glyph = glyph;
    public readonly RGB Foreground = foreground;
    public readonly RGB Background = background;

    public static Etchel Empty => default;
    public static Etchel Solid(RGB color) => new(Glyph.Empty, RGB.Black, color);

    public bool Equals(Etchel other) => Intrinsics.Equality<Etchel, ulong>(this, other);
    public override bool Equals(object? obj) => obj is Etchel other && Equals(other);
    public static bool operator ==(Etchel left, Etchel right) => left.Equals(right);
    public static bool operator !=(Etchel left, Etchel right) => !(left == right);
    public override int GetHashCode() => Intrinsics.Hashcode<Etchel, ulong>(this);
    public override string ToString() => $"'{(char)Glyph.Codepoint}' fg={Foreground} bg={Background}";
}