namespace Etch.Common;

public readonly struct Rect(Int2 position, Int2 size)
{
    public readonly Int2 Position = position;
    public readonly Int2 Size = size;

    public readonly Int2 Start => Position; // Inclusive Start
    public readonly Int2 End => Position + Size; // Exclusive End

    public static Rect Empty => new(Int2.Zero, Int2.Zero);
    public Rect? Interior => Size > Int2.Two ? new(Start + 1, Size - 2) : null;

    public bool Contains(Int2 point) => Start <= point && point < End;
    public bool Contains(Rect rect) => Contains(rect.Start) && Contains(rect.End);
}