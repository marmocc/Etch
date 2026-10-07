namespace Etch.Common;

public readonly struct Flat(int value) : IEquatable<Flat>
{
    public readonly int Value = value;

    public static Flat None => new(-1);
    public static Flat Zero => new(0);

    public Int2 Unflatten(int stride) { var (q, r) = Math.DivRem(Value, stride); return new(r, q); }
    public Int2 Unflatten(Int2 size) { var (q, r) = Math.DivRem(Value, size.X); return new(r, q); }
    public bool IsWithin(Int2 size) => (uint)Value < (uint)(size.X * size.Y);

    public bool Equals(Flat other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is Flat flat && Equals(flat);
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Flat left, Flat right) => left.Equals(right);
    public static bool operator ==(Flat left, int right) => left.Value.Equals(right);
    public static bool operator !=(Flat left, Flat right) => !(left == right);
    public static bool operator !=(Flat left, int right) => !(left == right);
    public static bool operator >(Flat left, Flat right) => left.Value > right.Value;
    public static bool operator >(Flat left, int right) => left.Value > right;
    public static bool operator <(Flat left, Flat right) => left.Value < right.Value;
    public static bool operator <(Flat left, int right) => left.Value < right;
    public static bool operator >=(Flat left, Flat right) => left.Value >= right.Value;
    public static bool operator >=(Flat left, int right) => left.Value >= right;
    public static bool operator <=(Flat left, Flat right) => left.Value <= right.Value;
    public static bool operator <=(Flat left, int right) => left.Value <= right;

    public static Flat operator +(Flat left, int right) => new(left.Value + right);
    public static Flat operator +(Flat left, Flat right) => new(left.Value + right.Value);
    public static Flat operator ++(Flat flat) => new(flat.Value + 1);
    public static Flat operator -(Flat left, int right) => new(left.Value - right);
    public static Flat operator -(Flat left, Flat right) => new(left.Value - right.Value);
    public static Flat operator --(Flat flat) => new(flat.Value - 1);
}