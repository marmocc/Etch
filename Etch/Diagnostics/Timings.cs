using System.Diagnostics;

namespace Etch.Diagnostics;

public sealed class Timings
{
    private long _origin, _mark;
    private TimeSpan _paint, _present, _flush, _wait;

    public TimeSpan Elapsed => Stopwatch.GetElapsedTime(_origin, _mark);

    public TimeSpan Paint => _paint;
    public TimeSpan Present => _present;
    public TimeSpan Flush => _flush;
    public TimeSpan Wait => _wait;

    public TimeSpan Busy => _paint + _present + _flush;
    public TimeSpan Frame => _paint + _present + _flush + _wait;
    public double Framerate => Frame > TimeSpan.Zero ? 1 / Frame.TotalSeconds : 0;
    public double PotentialFramerate => Busy > TimeSpan.Zero ? 1 / Busy.TotalSeconds : 0;

    public void Begin() => _origin = _mark = Stopwatch.GetTimestamp();
    public Probe MeasurePaint() => new(ref _mark, ref _paint);
    public Probe MeasurePresent() => new(ref _mark, ref _present);
    public Probe MeasureFlush() => new(ref _mark, ref _flush);
    public Probe MeasureWait() => new(ref _mark, ref _wait);
}
