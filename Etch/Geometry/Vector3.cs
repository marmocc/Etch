using System.Numerics;

namespace Etch.Geometry;

public readonly struct Vector3<T>(T x, T y, T z) :
    IAdditionOperators<Vector3<T>, Vector3<T>, Vector3<T>>,
    ISubtractionOperators<Vector3<T>, Vector3<T>, Vector3<T>>,
    IMultiplyOperators<Vector3<T>, T, Vector3<T>>,
    IEquatable<Vector3<T>> where T : INumber<T>
{
    public readonly T X = x, Y = y, Z = z;

    // Equality and comparison
    public bool Equals(Vector3<T> other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    public override bool Equals(object? obj) => obj is Vector3<T> vec && Equals(vec);
    public static bool operator ==(Vector3<T> left, Vector3<T> right) => left.Equals(right);
    public static bool operator !=(Vector3<T> left, Vector3<T> right) => !left.Equals(right);
    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    // Arithmetic operators
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    public static Vector3<T> operator *(Vector3<T> left, T right) => new(left.X * right, left.Y * right, left.Z * right);
    public static Vector3<T> operator *(T left, Vector3<T> right) => new(left * right.X, left * right.Y, left * right.Z);

    // Conversion operators
    public static implicit operator Vector3<T>((T x, T y, T z) source) => new(source.x, source.y, source.z);
    public static implicit operator (T x, T y, T z)(Vector3<T> source) => (source.X, source.Y, source.Z);

    // Common vectors
    public static Vector3<T> Zero => new(T.Zero, T.Zero, T.Zero);
    public static Vector3<T> One => new(T.One, T.One, T.One);
}
