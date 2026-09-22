using System.Numerics;

namespace Etch.Geometry;

public readonly struct Rectangle2D<T>(Vector2<T> position, Vector2<T> size) where T : INumber<T>
{
    public readonly Vector2<T> Position = position;
    public readonly Vector2<T> Size = size;

    public bool Contains(Vector2<T> point) =>
        point.X >= Position.X && point.X < Position.X + Size.X &&
        point.Y >= Position.Y && point.Y < Position.Y + Size.Y;
}
