namespace Etch.Backend;

public readonly ref struct Pace(TimeSpan deadline) : IEquatable<Pace>
{
    private readonly TimeSpan _deadline = deadline;

    public static Pace Continuous => new(TimeSpan.Zero);
    public static Pace Indefinite => new(TimeSpan.MaxValue);
    public static Pace At(TimeSpan deadline) => new(deadline);

    public TimeSpan Remaining(TimeSpan now)
    {
        if (Equals(Indefinite)) return Timeout.InfiniteTimeSpan;
        return _deadline > now ? _deadline - now : TimeSpan.Zero;
    }

    public bool Equals(Pace other) => _deadline == other._deadline;
}
