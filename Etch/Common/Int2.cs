using System.Runtime.InteropServices;

namespace Etch.Common;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public readonly struct Int2(int x, int y) : IEquatable<Int2>
{
    [FieldOffset(0)] public readonly int X = x;
    [FieldOffset(4)] public readonly int Y = y;

    [FieldOffset(0)] public readonly long XY;

    public static Int2 Zero => new(0, 0);
    public static Int2 One => new(1, 1);

    public Int2 WithX(int x) => new(x, Y);
    public Int2 WithY(int y) => new(X, y);
    public Int2 AddX(int increment) => new(X + increment, Y);
    public Int2 AddY(int increment) => new(X, Y + increment);

    public bool Equals(Int2 other) => this.XY == other.XY;
    public override bool Equals(object? obj) => obj is Int2 int2 && Equals(int2);
    public override int GetHashCode() => XY.GetHashCode();

    public static bool operator ==(Int2 left, Int2 right) => left.Equals(right);
    public static bool operator !=(Int2 left, Int2 right) => !(left == right);
    public static bool operator >(Int2 left, Int2 right) => left.X > right.X && left.Y > right.Y;
    public static bool operator <(Int2 left, Int2 right) => left.X < right.X && left.Y < right.Y;
    public static bool operator >=(Int2 left, Int2 right) => left.X >= right.X && left.Y >= right.Y;
    public static bool operator <=(Int2 left, Int2 right) => left.X <= right.X && left.Y <= right.Y;

    public static Int2 operator +(Int2 left, Int2 right) => new(left.X + right.X, right.Y + right.Y);
    public static Int2 operator +(Int2 left, int right) => new(left.X + right, left.Y + right);
    public static Int2 operator *(Int2 left, int right) => new(left.X * right, left.Y * right);
}