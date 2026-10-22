using Etch.Common;

namespace Etch.Rendering.ANSI;

public sealed class Surface : IDispatcher, IDisposable
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed;

    private Int2 _cursor;
    private RGB _foreground;
    private RGB _background;

    private int _written;
    private readonly byte[] _buffer;
    private readonly Etchel[] _shadow;
    private readonly Stream _output;

    public Int2 Size { get; }
    public bool Stopping => _stopping;

    public Surface(Int2 size, Stream output)
    {
        Size = size;

        _written = 0;
        _buffer = new byte[size.X * size.Y * 64];
        _shadow = new Etchel[size.X * size.Y];
        _output = output;

        _cursor = Int2.Zero;
        _foreground = RGB.Black;
        _background = RGB.Black;

        Sequence.Move(_cursor, _buffer, ref _written);
        Sequence.Foreground(_foreground, _buffer, ref _written);
        Sequence.Background(_background, _buffer, ref _written);
        Present();
    }

    public Surface() : this(new(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput())
    {
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; Stop(); };
        Sequence.Alternate(true, _buffer, ref _written);
        Sequence.Cursor(false, _buffer, ref _written);
        Sequence.Clear(_buffer, ref _written);
        Present();
    }

    public void Dispatch(Context context)
    {
        foreach (var command in context.FullscreenBlits)
        {
            foreach(var etchel in context.Etchels.Access(command.Handle))
            {
                Sequence.Foreground(etchel.Foreground, _buffer, ref _written, ref _foreground);
                Sequence.Background(etchel.Background, _buffer, ref _written, ref _background);
                Sequence.Write(etchel.Glyph.Codepoint, _buffer, ref _written, ref _cursor);
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
        Sequence.Alternate(false, _buffer, ref _written);
        Sequence.Cursor(true, _buffer, ref _written);
        Sequence.Reset(_buffer, ref _written);
        Sequence.Clear(_buffer, ref _written);
        Present();
    }
}