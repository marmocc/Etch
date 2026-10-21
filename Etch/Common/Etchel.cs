namespace Etch.Common;

public readonly record struct Etchel(Glyph Glyph, RGB Foreground, RGB Background)
{
    /// <summary>
    /// <c>Glyph</c> is a byte and <c>RGB</c> is 3 bytes, so <c>Etchel</c> is 7 bytes without padding.
    /// <c>_padding</c> is needed for <c>Etchel</c> to reach 8 bytes and be casted into a <c>ulong</c>.
    /// </summary>
    private readonly byte _padding;

    public static Etchel Empty => default;
    public static Etchel Solid(RGB fill) => new(Glyph.Empty, Color.Black, fill);
}