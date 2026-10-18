using Etch.Common;
using Etch.Diagnostics;
using System.Runtime.InteropServices;

namespace Etch.Backend.ANSI;

public sealed class Surface(Int2 size, Stream output) : ISurface
{
    private readonly AutoResetEvent _wake = new(false);
    private volatile bool _stopping = false;

    private readonly Writer _writer = new(size.X * size.Y * 64, output);
    private readonly Etchel[] _current = new Etchel[size.X * size.Y];
    private readonly Etchel[] _shadow = new Etchel[size.X * size.Y];

    public Int2 Size { get; } = size;
    public Timings Timings { get; } = new();

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
        if (pace == Pace.Continuous) return;
        else if (pace == Pace.Indefinite) _wake.WaitOne();
        else _wake.WaitOne(pace.Interval);
    }

    public void Start<TPainter>(TPainter painter) where TPainter : IPainter, allows ref struct
    {
        ulong frames = 0;
        Pace pace = painter.Pace;
        Timings.Begin();
        while(!_stopping)
        {
            Context context = new(frames, Timings.Elapsed, Timings.Frame, Size, _current);
            using (Timings.MeasurePaint()) painter.Paint(context);
            using (Timings.MeasurePresent()) Present();
            using (Timings.MeasureFlush()) _writer.Flush();
            using (Timings.MeasureWait()) Wait(pace);
            frames++;
        }
    }

    public void Wake() => _wake.Set();
    public void Stop() { _stopping = true; Wake(); }
}