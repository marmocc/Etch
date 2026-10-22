using Etch.Common;

namespace Etch.Rendering.ANSI;

public sealed class Surface(Int2 size, Stream output) : IDispatcher, IDisposable
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed;

    private int _written = 0;
    private readonly byte[] _buffer = new byte[size.X * size.Y * 64];
    private readonly Etchel[] _shadow = new Etchel[size.X * size.Y];
    private readonly Stream _output = output;

    public Int2 Size { get; } = size;
    public bool Stopping => _stopping;

    public Surface() : this(new(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput())
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; Stop(); };
        Console.CursorVisible = false;
    }

    public void Dispatch(Context context)
    {
        Int2 cursor = Int2.Zero;
        Sequence.Move(cursor, _buffer, ref _written);
        RGB foreground = RGB.Black;
        Sequence.Foreground(foreground, _buffer, ref _written);
        RGB background = RGB.Black;
        Sequence.Background(background, _buffer, ref _written);

        foreach (var command in context.FullscreenBlits)
        {
            Span<Etchel> etchels = context.Etchels.Access(command.Handle);
            foreach(var etchel in etchels)
            {
                if(foreground != etchel.Foreground) Sequence.Foreground(etchel.Foreground, _buffer, ref _written);
                if(background != etchel.Background) Sequence.Background(etchel.Background, _buffer, ref _written);
                if(!etchel.Glyph.IsPrintable) _buffer[_written++] = etchel.Glyph.Codepoint;
            }
        }
    }

    public void Present()
    {
        _output.Write(_buffer.AsSpan(0, _written));
        _written = 0;
    }
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