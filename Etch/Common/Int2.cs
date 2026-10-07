namespace Etch.Common;

public readonly struct Int2(int x, int y) : IEquatable<Int2>
{
    public readonly int X = x;
    public readonly int Y = y;

    public static Int2 Zero => new(0, 0);
    public static Int2 One => new(1, 1);
    public static Int2 Two => new(2, 2);

    public Int2 WithX(int x) => new(x, Y);
    public Int2 WithY(int y) => new(X, y);

    public bool Equals(Int2 other) => X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => obj is Int2 int2 && Equals(int2);
    public override int GetHashCode() => HashCode.Combine(X, Y);

    public Flat Flatten(int stride) => new(Y * stride + X);
    public Flat Flatten(Int2 size) => new(Y * size.X + X);

    public bool AllLess(Int2 other) => X < other.X && Y < other.Y;
    public bool AllLessOrEqual(Int2 other) => X <= other.X && Y <= other.Y;
    public bool AllGreater(Int2 other) => X > other.X && Y > other.Y;
    public bool AllGreaterOrEqual(Int2 other) => X >= other.X && Y >= other.Y;

    public static bool operator ==(Int2 left, Int2 right) => left.Equals(right);
    public static bool operator !=(Int2 left, Int2 right) => !(left == right);

    public static Int2 operator +(Int2 left, Int2 right) => new(left.X + right.X, left.Y + right.Y);
    public static Int2 operator +(Int2 left, int right) => new(left.X + right, left.Y + right);
    public static Int2 operator -(Int2 left, Int2 right) => new(left.X - right.X, left.Y - right.Y);
    public static Int2 operator -(Int2 left, int right) => new(left.X - right, left.Y - right);
    public static Int2 operator *(Int2 left, int right) => new(left.X * right, left.Y * right);
}