using Etch.Common;

namespace Etch.Rendering.ANSI;

public sealed class Surface : ISurface
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed = false;

    private int _written;
    private readonly byte[] _buffer;
    private readonly Stream _output;

    public Span<byte> Buffer => _buffer;
    public bool Stopping => _stopping;
    public Int2 Size { get; }

    public Int2 Cursor
    {
        get;
        set
        {
            if (value == field) return;
            new Sequence.Move(value).Write(_buffer, ref _written);
            field = value;
        }
    }

    public RGB Foreground
    {
        get;
        set
        {
            if (value == field) return;
            Sequence.Color.Foreground(value).Write(_buffer, ref _written);
            field = value;
        }
    }

    public RGB Background
    {
        get;
        set
        {
            if (value == field) return;
            Sequence.Color.Background(value).Write(_buffer, ref _written);
            field = value;
        }
    }

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
        Sequence.Alternate.Enter().Write(_buffer, ref _written);
        Sequence.Cursor.Hide().Write(_buffer, ref _written);
        new Sequence.Clear().Write(_buffer, ref _written);

        Initialize();
        Flush();
    }

    private void OnCancelKeyPress(object? first, ConsoleCancelEventArgs e) { e.Cancel = true; Stop(); }

    private void Initialize()
    {
        new Sequence.Move(Cursor).Write(_buffer, ref _written);
        Sequence.Color.Foreground(Foreground).Write(_buffer, ref _written);
        Sequence.Color.Background(Background).Write(_buffer, ref _written);
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
        new Sequence.Clear().Write(_buffer, ref _written);
        Sequence.Alternate.Leave().Write(_buffer, ref _written);
        Sequence.Cursor.Show().Write(_buffer, ref _written);
        new Sequence.Reset().Write(_buffer, ref _written);
        Flush();

        _wake.Dispose();
    }
}