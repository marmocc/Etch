using Etch.Common;
using System.Diagnostics;

namespace Etch.Diagnostics;

/// <summary>
/// Measures time passed inside a <c>using</c> scope.
/// </summary>
/// <param name="mark">Starting timestamp, but also updated at <c>Dispose</c>.</param>
/// <param name="duration">Output of the <c>Probe</c> measurement.</param>
public readonly ref struct Probe(ref long mark, ref TimeSpan duration) : IDisposable
{
    private readonly long _start = mark;
    private readonly ref long _mark = ref mark;
    private readonly ref TimeSpan _duration = ref duration;

    public void Dispose()
    {
        long end = Stopwatch.GetTimestamp();
        _duration = Stopwatch.GetElapsedTime(_start, end);
        _mark = end;
    }
}