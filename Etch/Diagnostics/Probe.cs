using System.Diagnostics;

namespace Etch.Diagnostics;

/// <summary>
/// Measures time passed inside a <c>using</c> scope.
/// </summary>
/// <param name="mark">Starting timestamp.</param>
/// <param name="duration">Duration measurement.</param>
public readonly ref struct Probe(ref long mark, ref TimeSpan duration) : IDisposable
{
    private readonly long _start = mark;
    private readonly ref long _mark = ref mark;
    private readonly ref TimeSpan _duration = ref duration;

    /// <summary>
    /// Finishes measurement and sets mark and duration.
    /// </summary>
    public void Dispose()
    {
        long end = Stopwatch.GetTimestamp();
        _duration = Stopwatch.GetElapsedTime(_start, end);
        _mark = end;
    }
}