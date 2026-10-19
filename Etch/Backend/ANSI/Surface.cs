using Etch.Common;
using System.Runtime.InteropServices;

namespace Etch.Backend.ANSI;

public sealed class Surface(Int2 size, Stream output) : ISurface, IDisposable
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;
    private bool _disposed;

    private readonly Writer _writer = new(size.X * size.Y * 64, output);
    private readonly Etchel[] _shadow = new Etchel[size.X * size.Y];

    public Int2 Size { get; } = size;
    public bool Stopping => _stopping;

    public static Surface Default()
    {
        Stream terminalStream = Console.OpenStandardOutput();
        Int2 terminalSize = new(Console.WindowWidth, Console.WindowHeight);
        Surface surface = new(terminalSize, terminalStream);
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; surface.Stop(); };
        Console.CursorVisible = false;
        return surface;
    }

    public void Present(ReadOnlySpan<Etchel> current)
    {
        Span<Etchel> shadow = _shadow;
        ReadOnlySpan<ulong> currentAsULong = MemoryMarshal.Cast<Etchel, ulong>(current);
        ReadOnlySpan<ulong> shadowAsULong = MemoryMarshal.Cast<Etchel, ulong>(shadow);
        if (currentAsULong.SequenceEqual(shadowAsULong)) return;

        for (int i = 0; i < current.Length; i++)
        {
            Etchel etchel = current[i];
            if (etchel == shadow[i]) continue;

            shadow[i] = etchel;
            Int2 position = new Flat(i).Unflatten(Size);
            _writer.Move(position);
            _writer.Foreground(etchel.Foreground);
            _writer.Background(etchel.Background);
            _writer.Write(etchel.Glyph.Codepoint);
        }

        _writer.Flush();
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