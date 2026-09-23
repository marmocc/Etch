using System.Numerics;

namespace Etch.Geometry;

public readonly struct Vector2<T>(T x, T y) :
    IAdditionOperators<Vector2<T>, Vector2<T>, Vector2<T>>,
    ISubtractionOperators<Vector2<T>, Vector2<T>, Vector2<T>>,
    IMultiplyOperators<Vector2<T>, T, Vector2<T>>,
    IEquatable<Vector2<T>> where T : INumber<T>
{
    public readonly T X = x, Y = y;

    // Equality and comparison
    public bool Equals(Vector2<T> other) => X.Equals(other.X) && Y.Equals(other.Y);
    public override bool Equals(object? obj) => obj is Vector2<T> vec && Equals(vec);
    public static bool operator ==(Vector2<T> left, Vector2<T> right) => left.Equals(right);
    public static bool operator !=(Vector2<T> left, Vector2<T> right) => !left.Equals(right);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    // Arithmetic operators
    public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right) => new(left.X + right.X, left.Y + right.Y);
    public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right) => new(left.X - right.X, left.Y - right.Y);
    public static Vector2<T> operator *(Vector2<T> left, T right) => new(left.X * right, left.Y * right);
    public static Vector2<T> operator *(T left, Vector2<T> right) => new(left * right.X, left * right.Y);

    // Conversion operators
    public static implicit operator Vector2<T>((T x, T y) source) => new(source.x, source.y);
    public static implicit operator (T x, T y)(Vector2<T> source) => (source.X, source.Y);

    // Common vectors
    public static Vector2<T> Zero => new(T.Zero, T.Zero);
    public static Vector2<T> One => new(T.One, T.One);
}
