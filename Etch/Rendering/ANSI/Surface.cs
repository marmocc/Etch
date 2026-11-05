using Etch.Common;
using Etch.Rendering.ANSI.Sequence;
using System.Runtime.CompilerServices;

namespace Etch.Rendering.ANSI;

public sealed class Surface : ISurface
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed = false;

    private int _written;
    private readonly byte[] _buffer;
    private readonly Stream _output;

    private Int2 _cursor;
    private RGB _foreground;
    private RGB _background;

    public bool Stopping => _stopping;
    public Int2 Size { get; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetCursor(Int2 position)
    {
        if (position == _cursor) return;
        new Move(position).Write(_buffer, ref _written);
        _cursor = position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AssumeCursor(Int2 position) => _cursor = position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetForeground(RGB color)
    {
        if (color == _foreground) return;
        Color.Foreground(color).Write(_buffer, ref _written);
        _foreground = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetBackground(RGB color)
    {
        if (color == _background) return;
        Color.Background(color).Write(_buffer, ref _written);
        _background = color;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Put(byte codepoint) => _buffer[_written++] = codepoint;

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
        Present();
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
        Present();
    }

    private void OnCancelKeyPress(object? first, ConsoleCancelEventArgs e) { e.Cancel = true; Stop(); }

    private void Initialize()
    {
        new Move(_cursor).Write(_buffer, ref _written);
        Color.Foreground(_foreground).Write(_buffer, ref _written);
        Color.Background(_background).Write(_buffer, ref _written);
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

        Console.CancelKeyPress -= OnCancelKeyPress;
        new Clear().Write(_buffer, ref _written);
        Alternate.Leave().Write(_buffer, ref _written);
        Cursor.Show().Write(_buffer, ref _written);
        new Reset().Write(_buffer, ref _written);
        Present();

        _wake.Dispose();
    }
}
