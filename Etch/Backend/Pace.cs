namespace Etch.Backend;

public readonly struct Pace(TimeSpan interval) : IEquatable<Pace>
{
    public readonly TimeSpan Interval = interval;

    public static Pace Continuous => new(TimeSpan.Zero);
    public static Pace Indefinite => new(TimeSpan.MaxValue);
    public static Pace Every(TimeSpan interval) => new(interval);
    public static Pace Every(double interval) => new(TimeSpan.FromSeconds(interval));
    public static Pace Target(int fps) => new(TimeSpan.FromSeconds(1.0 / fps));

    public bool Equals(Pace other) => Interval == other.Interval;
    public override bool Equals(object? obj) => obj is Pace other && Equals(other);
    public static bool operator ==(Pace left, Pace right) => left.Equals(right);
    public static bool operator !=(Pace left, Pace right) => !(left == right);
    public override int GetHashCode() => Interval.GetHashCode();
}
