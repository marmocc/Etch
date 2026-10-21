namespace Etch.Backend;

public readonly ref struct Timings(ulong frame, TimeSpan elapsed, TimeSpan delta)
{
    public readonly ulong Frame = frame;
    public readonly TimeSpan Elapsed = elapsed;
    public readonly TimeSpan Delta = delta;
}
