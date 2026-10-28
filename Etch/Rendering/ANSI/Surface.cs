using Etch.Common;

namespace Etch.Rendering.ANSI;

public sealed class Surface : ISurface
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed = false;

    private Int2 _cursor;
    private RGB _foreground;
    private RGB _background;

    private int _written;
    private readonly byte[] _buffer;
    private readonly Stream _output;

    public Int2 Size { get; }
    public bool Stopping => _stopping;

    /// <summary>
    /// Constructor for a generic ANSI stream output
    /// </summary>
    public Surface(Int2 size, Stream output)
    {
        Size = size;

        _written = 0;
        _buffer = new byte[Size.X * Size.Y * 64];
        _output = output;

        Initialize();
        Flush();
    }

    /// <summary>
    /// Constructor already set for the Terminal output
    /// </summary>
    public Surface()
    {
        Size = new(Console.WindowWidth, Console.WindowHeight);

        _written = 0;
        _buffer = new byte[Size.X * Size.Y * 64];
        _output = Console.OpenStandardOutput();

        Console.CancelKeyPress += OnCancelKeyPress;
        Sequence.Alternate(true, _buffer, ref _written);
        Sequence.Cursor(false, _buffer, ref _written);
        Sequence.Clear(_buffer, ref _written);

        Initialize();
        Flush();
    }

    private void OnCancelKeyPress(object? first, ConsoleCancelEventArgs e) { e.Cancel = true; Stop(); }

    private void Initialize()
    {
        _cursor = Int2.Zero;
        _foreground = RGB.Black;
        _background = RGB.Black;
        Sequence.Move(_cursor, _buffer, ref _written);
        Sequence.Foreground(_foreground, _buffer, ref _written);
        Sequence.Background(_background, _buffer, ref _written);
    }

    public void Flush()
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

        Console.CancelKeyPress -= OnCancelKeyPress;
        Sequence.Clear(_buffer, ref _written);
        Sequence.Alternate(false, _buffer, ref _written);
        Sequence.Cursor(true, _buffer, ref _written);
        Sequence.Reset(_buffer, ref _written);
        Flush();

        _wake.Dispose();
    }

    public void Present(Batch frame)
    {
        foreach (ref readonly Packet command in frame.Packets)
        {
            switch (command.Command)
            {
                case Command.FullscreenBlit:
                {
                    ReadOnlySpan<Etchel> etchels = frame.Assets.Access<Etchel>(command.Handle);
                    Sequence.Move(Int2.Zero, _buffer, ref _written);
                    foreach (var (glyph, foreground, background) in etchels)
                    {
                        Sequence.Foreground(foreground, _buffer, ref _written, ref _foreground);
                        Sequence.Background(background, _buffer, ref _written, ref _background);
                        Sequence.Write(glyph.Codepoint, _buffer, ref _written); // No need to update cursor in a FullscreenBlit.
                    }
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }
    }
}