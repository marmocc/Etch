namespace Etch.Graphics;

public readonly struct Vector2<T>(T x, T y) : IEquatable<Vector2<T>> where T : IEquatable<T>
{
    public T X { get; } = x;
    public T Y { get; } = y;

    public bool Equals(Vector2<T> other) => X.Equals(other.X)
                                         && Y.Equals(other.Y);
}
