namespace Etch.Common;

public readonly record struct Rect(Int2 Position, Int2 Size)
{
    /// <summary>
    /// Point where the Rect inclusively starts from.
    /// </summary>
    public readonly Int2 Start => Position;

    /// <summary>
    /// Point where the Rect exclusively ends at.
    /// </summary>
    public readonly Int2 End => Position + Size;

    public static Rect Empty => new(Int2.Zero, Int2.Zero);
    public Rect? Interior => Size.AllGreater(Int2.Two) ? new(Position + 1, Size - 2) : null;

    public bool Contains(Int2 point) => point.AllGreaterOrEqual(Start) && point.AllLess(End);
    public bool Contains(Rect rect) => rect.Start.AllGreaterOrEqual(Start) && rect.End.AllLessOrEqual(End);
}