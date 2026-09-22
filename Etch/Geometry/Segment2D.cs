using System.Numerics;

namespace Etch.Geometry;

public readonly struct Segment2D<T>(Vector2<T> start, Vector2<T> end) where T : INumber<T>
{
    public readonly Vector2<T> Start = start;
    public readonly Vector2<T> End = end;

    public Vector2<T> Direction => End - Start;
}
