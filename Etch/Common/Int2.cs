namespace Etch.Common;

public readonly record struct Int2(int X, int Y)
{
    public static Int2 Zero => new(0, 0);
    public static Int2 One => new(1, 1);
    public static Int2 Two => new(2, 2);

    public static int Flatten(int x, int y) => x * y;
    public static int Flatten(int x, int y, int stride) => y * stride + x;

    public int Flatten() => Flatten(X, Y);
    public int Flatten(int stride) => Flatten(X, Y, stride);
    public int Flatten(Int2 size) => Flatten(X, Y, size.X);

    public bool AllLess(Int2 other) => X < other.X && Y < other.Y;
    public bool AllLessOrEqual(Int2 other) => X <= other.X && Y <= other.Y;
    public bool AllGreater(Int2 other) => X > other.X && Y > other.Y;
    public bool AllGreaterOrEqual(Int2 other) => X >= other.X && Y >= other.Y;

    public static Int2 operator +(Int2 left, Int2 right) => new(left.X + right.X, left.Y + right.Y);
    public static Int2 operator +(Int2 left, int right) => new(left.X + right, left.Y + right);
    public static Int2 operator -(Int2 left, Int2 right) => new(left.X - right.X, left.Y - right.Y);
    public static Int2 operator -(Int2 left, int right) => new(left.X - right, left.Y - right);
    public static Int2 operator *(Int2 left, int right) => new(left.X * right, left.Y * right);
}