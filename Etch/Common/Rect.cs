namespace Etch.Common;

public readonly struct Rect(Int2 position, Int2 size)
{
    public readonly Int2 Position = position;
    public readonly Int2 Size = size;

    public Int2 Start => Position;
    public Int2 End => Position + Size;

    public bool Contains(Int2 point) => Start < point && point < End;
    public bool Contains(Rect rect) => Contains(rect.Start) && Contains(rect.End);
}
