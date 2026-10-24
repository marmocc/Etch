namespace Etch.Common;

public readonly record struct Glyph(ushort Index)
{
    public bool IsEmpty => Index == 0;
    public bool IsBlank => Codepoint == (byte)' ';
    public bool IsPrintable => (uint)(Index - 32) < 95;
    public byte Codepoint => IsPrintable ? (byte)Index : (byte)' ';

    public static Glyph Empty => default;
    public static Glyph From(char c)
    {
        if ((uint)(c - 32) >= 95) throw new ArgumentOutOfRangeException(nameof(c), "Character must be in the 32-126 ASCII range.");
        return new Glyph((byte)c);
    }
}
