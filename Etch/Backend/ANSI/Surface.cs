using Etch.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Etch.Backend.ANSI;

public sealed class Surface(Int2 size, Stream output) : ISurface
{
    private readonly long _start = Stopwatch.GetTimestamp();
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;

    private readonly Writer _writer = new(size.X * size.Y * 64, output);
    private readonly Etchel[] _current = new Etchel[size.X * size.Y];
    private readonly Etchel[] _shadow = new Etchel[size.X * size.Y];

    public Int2 Size { get; } = size;

    public static Surface Terminal()
    {
        Stream terminalStream = Console.OpenStandardOutput();
        Int2 terminalSize = new(Console.WindowWidth, Console.WindowHeight);
        Surface surface = new(terminalSize, terminalStream);
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; surface.Stop(); };
        Console.CursorVisible = false;
        return surface;
    }

    private void Present()
    {
        ReadOnlySpan<Etchel> current = _current;
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
    }

    private void Wait(Pace pace)
    {
        TimeSpan elapsed = Stopwatch.GetElapsedTime(_start);
        TimeSpan remaining = pace.Remaining(elapsed);
        if (remaining == TimeSpan.Zero) return;
        _wake.WaitOne(remaining);
    }

    public void Start<TPainter>(TPainter painter) where TPainter : IPainter, allows ref struct
    {
        TimeSpan previousElapsed = TimeSpan.Zero;
        ulong frames = 0;
        while(!_stopping)
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(_start);
            TimeSpan delta = elapsed - previousElapsed;
            previousElapsed = elapsed;

            Context context = new(frames, elapsed, delta, Size, _current);
            Pace pace = painter.Paint(context);
            frames++;

            Present();
            _writer.Flush();
            Wait(pace);
        }
    }

    public void Wake() => _wake.Set();
    public void Stop() { _stopping = true; Wake(); }
}