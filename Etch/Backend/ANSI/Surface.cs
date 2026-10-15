using Etch.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Etch.Backend.ANSI;

public sealed class Surface(Int2 size, Stream stream) : ISurface
{
    private readonly Writer _writer = new(size.X * size.Y * 64, stream);
    
    private Etchel[] _current = new Etchel[size.X * size.Y];
    private Etchel[] _shadow = new Etchel[size.X * size.Y];

    public static Surface Default => new(new Int2(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput());

    public static ReadOnlySpan<byte> Ramp => " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
    public static byte Density(RGBA color) => Ramp[color.Luminance * Ramp.Length >> 8];

    public Int2 Size { get; } = size;

    private void Swap() => (_current, _shadow) = (_shadow, _current);
    private void Present()
    {
        ReadOnlySpan<Etchel> current = _current;
        ReadOnlySpan<Etchel> shadow = _shadow;

        ReadOnlySpan<ulong> currentAsULong = MemoryMarshal.Cast<Etchel, ulong>(current);
        ReadOnlySpan<ulong> shadowAsULong = MemoryMarshal.Cast<Etchel, ulong>(shadow);
        if(currentAsULong.SequenceEqual(shadowAsULong)) return;

        for (int i = 0; i < current.Length; i++)
        {
            Etchel etchel = current[i];
            if (etchel == shadow[i]) continue;
            Int2 position = new Flat(i).Unflatten(Size);
            _writer.Move(position);
            _writer.Foreground(etchel.Foreground);
            _writer.Background(etchel.Background);
            _writer.Write(etchel.Glyph.Codepoint);
        }
    }

    public void Run<TPainter>(TPainter painter, int until = 0) where TPainter : IPainter, allows ref struct
    {
        _writer.Clear();
        Array.Clear(_shadow);

        long frame = 0;
        long start = Stopwatch.GetTimestamp();
        long last = start;

        while (true)
        {
            long now = Stopwatch.GetTimestamp();
            double elapsed = (now - start) / (double)Stopwatch.Frequency;
            float delta = (float)((now - last) / (double)Stopwatch.Frequency);
            last = now;

            Array.Clear(_current);
            Context context = new(frame, elapsed, delta, Size, _current);
            painter.Paint(context);
            Present();
            Swap();

            _writer.Flush();
            frame++;

            if (until > 0 && frame >= until) break;
        }
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }
}