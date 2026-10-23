using Etch.Painters;
using Etch.Rendering;

namespace Etch.Diagnostics;

public readonly ref struct Benchmark<S, P>(S surface, P painter, Context context)
    where S : ISurface where P : IPainter, allows ref struct
{
    private readonly S _surface = surface;
    private readonly P _painter = painter;
    private readonly Context _context = context;

    public (TimeSpan, TimeSpan, TimeSpan) Run(int warmup = 100, int actual = 100)
    {
        TimeSpan paintSum = TimeSpan.Zero;
        TimeSpan presentSum = TimeSpan.Zero;
        TimeSpan flushSum = TimeSpan.Zero;

        for(int i = 0; i < warmup; i++)
        {
            TimeSpan paint = TimeSpan.Zero;
            TimeSpan present = TimeSpan.Zero;
            TimeSpan flush = TimeSpan.Zero;

            using (new Watch(ref paint)) _painter.Paint(_context);
            using (new Watch(ref present)) _surface.Present(_context);
            using (new Watch(ref flush)) _surface.Flush();
            _context.Clear();
        }

        for(int i = 0; i < actual; i++)
        {
            TimeSpan paint = TimeSpan.Zero;
            TimeSpan present = TimeSpan.Zero;
            TimeSpan flush = TimeSpan.Zero;

            using (new Watch(ref paint)) _painter.Paint(_context);
            using (new Watch(ref present)) _surface.Present(_context);
            using (new Watch(ref flush)) _surface.Flush();
            _context.Clear();

            paintSum += paint;
            presentSum += present;
            flushSum += flush;
        }

        return (paintSum / actual, presentSum / actual, flushSum / actual);
    }
}
