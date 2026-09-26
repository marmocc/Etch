namespace Etch.Common;

public readonly struct Rect(Int2 position, Int2 size)
{
    public readonly Int2 Position = position;
    public readonly Int2 Size = size;

    public Int2 Start => Position;
    public Int2 End => Position + Size - Int2.One;

    public bool Interior(Int2 point) => Start < point && point < End;
    public bool Closure(Int2 point) => Start <= point && point <= End;
    public bool Boundary(Int2 point) => Closure(point) && !Interior(point);

    public bool Interior(Rect rect) => Interior(rect.Start) && Interior(rect.End);
    public bool Closure(Rect rect) => Closure(rect.Start) && Closure(rect.End);
    public bool Boundary(Rect rect) => Boundary(rect.Start) && Boundary(rect.End);
}