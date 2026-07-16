namespace Etch.Common;

public readonly struct Glyph(byte index) : IEquatable<Glyph>
{
    public readonly byte Index = index;
    public bool IsEmpty => Index == 0;
    public bool IsBlank => Codepoint == (byte)' ';
    public bool IsPrintable => (uint)(Index - 32) < 95;
    public byte Codepoint => IsPrintable ? Index : (byte)' ';

    public static Glyph Empty => default;
    public static Glyph From(char c)
    {
        if ((uint)(c - 32) >= 95) throw new ArgumentOutOfRangeException(nameof(c), "Character must be in the 32-126 ASCII range.");
        return new Glyph((byte)c);
    }

    public bool Equals(Glyph other) => Index == other.Index;
    public override bool Equals(object? obj) => obj is Glyph other && Equals(other);
    public static bool operator ==(Glyph left, Glyph right) => left.Equals(right);
    public static bool operator !=(Glyph left, Glyph right) => !(left == right);
    public override int GetHashCode() => Index.GetHashCode();
    public override string ToString() => $"({Index}, '{(char)Codepoint}')";
}
