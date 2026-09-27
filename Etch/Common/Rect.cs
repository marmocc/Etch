namespace Etch.Common;

public readonly struct Rect(Int2 position, Int2 size)
{
    public readonly Int2 Position = position;
    public readonly Int2 Size = size;

    public readonly Int2 InclusiveStart => Position;
    public readonly Int2 ExclusiveEnd => Position + Size;

    public static Rect Empty => new(Int2.Zero, Int2.Zero);
    public Rect? Interior => Size > Int2.Two ? new(Position + 1, Size - 2) : null;

    public bool Contains(Int2 point) => InclusiveStart <= point && point < ExclusiveEnd;
    public bool Contains(Rect rect) => Contains(rect.InclusiveStart) && Contains(rect.ExclusiveEnd);
}