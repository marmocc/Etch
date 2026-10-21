using Etch.Backend.OpenGL;
using Etch.Common;

namespace Etch.Backend.ANSI;

public sealed class Surface(Int2 size, Stream output) : ISurface<Context>, IDisposable
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed;

    private readonly Writer _writer = new(size.X * size.Y * 64, output);
    private readonly Etchel[] _shadow = new Etchel[size.X * size.Y];

    public Int2 Size { get; } = size;
    public bool Stopping => _stopping;

    public Surface() : this(new(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput())
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; Stop(); };
        Console.CursorVisible = false;
    }

    public Context Context(Timings timings) => new(timings, Size, _shadow, _writer);
    public void Present() => _writer.Flush();
    public void Wait(TimeSpan interval)
    {
        if (interval == TimeSpan.Zero) return;
        if (interval == TimeSpan.MaxValue) _wake.WaitOne();
        else _wake.WaitOne(interval);
    }

    public void Wake() => _wake.Set();
    public void Stop() { _stopping = true; Wake(); }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        _wake.Dispose();
    }
}