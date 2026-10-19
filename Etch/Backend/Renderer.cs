using Etch.Common;
using Etch.Diagnostics;
using System.Diagnostics;

namespace Etch.Backend;

public static class Renderer
{
    public static void Start<TPainter>(ISurface surface, TPainter painter, TimeSpan interval) where TPainter : IPainter, allows ref struct
    {
        Etchel[] etchels = new Etchel[surface.Size.X * surface.Size.Y];
        long origin = Stopwatch.GetTimestamp();
        ulong frame = 0;

        long mark = origin;
        TimeSpan target = TimeSpan.Zero;
        TimeSpan paint = TimeSpan.Zero;
        TimeSpan present = TimeSpan.Zero;
        TimeSpan wait = TimeSpan.Zero;
        TimeSpan delta = TimeSpan.Zero;

        while(!surface.Stopping)
        {
            using (new Measure(ref mark, ref delta))
            {
                TimeSpan elapsed = Stopwatch.GetElapsedTime(origin, mark);
                TimeSpan remaining = target > elapsed ? target - elapsed : TimeSpan.Zero;
                using (new Measure(ref mark, ref wait))  surface.Wait(remaining);
                elapsed = Stopwatch.GetElapsedTime(origin, mark);

                if (elapsed >= target)
                {
                    target += interval;
                    Context context = new(frame, elapsed, delta, surface.Size, etchels);
                    using (new Measure(ref mark, ref paint)) painter.Paint(context);
                    using (new Measure(ref mark, ref present)) surface.Present(etchels);
                    frame++;
                }
            }
        }
    }
}