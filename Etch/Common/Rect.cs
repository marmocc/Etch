namespace Etch.Common;

public readonly record struct Rect(Int2 Position, Int2 Size)
{
    public readonly Int2 InclusiveStart => Position;
    public readonly Int2 ExclusiveEnd => Position + Size;

    public static Rect Empty => new(Int2.Zero, Int2.Zero);
    public Rect? Interior => Size.AllGreater(Int2.Two) ? new(Position + 1, Size - 2) : null;

    public bool Contains(Int2 point) => point.AllGreaterOrEqual(InclusiveStart) && point.AllLess(ExclusiveEnd);
    public bool Contains(Rect rect) => rect.InclusiveStart.AllGreaterOrEqual(InclusiveStart) && rect.ExclusiveEnd.AllLessOrEqual(ExclusiveEnd);
}