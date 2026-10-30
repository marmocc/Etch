using System.Diagnostics;

namespace Etch.Diagnostics;

/// <summary>
/// Measures time passed inside a <c>using</c> scope.
/// </summary>
public readonly ref struct Watch(ref TimeSpan duration) : IDisposable
{
    private readonly long _origin = Stopwatch.GetTimestamp();
    private readonly ref TimeSpan _duration = ref duration;

    /// <summary>
    /// Finishes measurement and sets the duration.
    /// </summary>
    public void Dispose()
    {
        long end = Stopwatch.GetTimestamp();
        _duration = Stopwatch.GetElapsedTime(_origin, end);
    }
}