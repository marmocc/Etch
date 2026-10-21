using Etch.Diagnostics;
using System.Diagnostics;

namespace Etch.Backend;

public static class Renderer
{
    public static void Start<TContext, TSurface, TPainter>(TimeSpan interval)
        where TContext : IContext, allows ref struct 
        where TSurface : ISurface<TContext>, new() 
        where TPainter : IPainter, new(), allows ref struct
    {
        using TSurface surface = new();
        TPainter painter = new();
        long origin = Stopwatch.GetTimestamp();
        ulong frame = 0;

        long mark = origin;
        TimeSpan target = TimeSpan.Zero;
        TimeSpan paint = TimeSpan.Zero;
        TimeSpan present = TimeSpan.Zero;
        TimeSpan wait = TimeSpan.Zero;
        TimeSpan delta = TimeSpan.Zero;

        while(true)
        {
            using (new Measure(ref mark, ref delta))
            {
                TimeSpan elapsed = Stopwatch.GetElapsedTime(origin, mark);
                TimeSpan remaining = target > elapsed ? target - elapsed : TimeSpan.Zero;
                using (new Measure(ref mark, ref wait)) surface.Wait(remaining);
                if (surface.Stopping) break;

                elapsed = Stopwatch.GetElapsedTime(origin, mark);

                if (elapsed >= target)
                {
                    target += interval;
                    Timings timings = new(frame, elapsed, delta);
                    TContext context = surface.Context(timings);
                    using (new Measure(ref mark, ref paint)) painter.Paint(context);
                    using (new Measure(ref mark, ref present)) surface.Present();
                    frame++;
                }
            }
        }
    }
}