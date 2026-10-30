using Etch.Common;
using Etch.Rendering.ANSI.Sequence;

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
        Alternate.Enter().Write(_buffer, ref _written);
        Cursor.Hide().Write(_buffer, ref _written);
        new Clear().Write(_buffer, ref _written);

        Initialize();
        Flush();
    }

    private void OnCancelKeyPress(object? first, ConsoleCancelEventArgs e) { e.Cancel = true; Stop(); }

    private void Initialize()
    {
        _cursor = Int2.Zero;
        _foreground = RGB.Black;
        _background = RGB.Black;
        new Move(_cursor).Write(_buffer, ref _written);
        Color.Foreground(_foreground).Write(_buffer, ref _written);
        Color.Background(_background).Write(_buffer, ref _written);
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
        new Clear().Write(_buffer, ref _written);
        Alternate.Leave().Write(_buffer, ref _written);
        Cursor.Show().Write(_buffer, ref _written);
        new Reset().Write(_buffer, ref _written);
        Flush();

        _wake.Dispose();
    }

    public void Present(Batch<Packet> packets, Arena assets)
    {
        foreach (ref readonly Packet packet in packets.Values)
        {
            switch (packet.Command)
            {
                case Command.FullscreenBlit:
                    ReadOnlySpan<Etchel> etchels = assets.Access<Etchel>(packet.Handle);
                    if (_cursor != Int2.Zero) { new Move(Int2.Zero).Write(_buffer, ref _written); _cursor = Int2.Zero; }
                    foreach (var (glyph, foreground, background) in etchels)
                    {
                        if(foreground != _foreground) { Color.Foreground(foreground).Write(_buffer, ref _written); _foreground = foreground; }
                        if(background != _background) { Color.Background(background).Write(_buffer, ref _written); _background = background; }
                        _buffer[_written++] = glyph.Codepoint;
                    }
                    _cursor = Size - 1;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}