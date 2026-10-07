using Etch.Common;
using System.Diagnostics;

namespace Etch.Backend.ANSI;

public sealed class Surface(Int2 size, Stream stream) : ISurface
{
    private readonly Writer _writer = new(size.X * size.Y * 64, stream);
    private readonly Color[] _back = new Color[size.X * size.Y];

    public static Surface Default => field ??= new(new Int2(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput());

    public Int2 Size { get; } = size;
    public void Run<TPainter>(TPainter painter, int until = 0) where TPainter : IPainter, allows ref struct
    {
        long frame = 0;
        long start = Stopwatch.GetTimestamp();
        long last = start;

        while (true)
        {
            long now = Stopwatch.GetTimestamp();
            double elapsed = (now - start) / (double)Stopwatch.Frequency;
            float delta = (float)((now - last) / (double)Stopwatch.Frequency);
            last = now;

            painter.Paint(new Context(frame, elapsed, delta, Size, _writer, _back));
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