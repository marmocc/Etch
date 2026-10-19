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

        while(!surface.Stopping)
        {
            TimeSpan elapsed = Stopwatch.GetElapsedTime(origin, mark);

            if(elapsed >= target)
            {
                target += interval;
                TimeSpan delta = paint + present + wait;
                Context context = new(frame, elapsed, delta, surface.Size, etchels);
                using (new Probe(ref mark, ref paint)) painter.Paint(context);
                using (new Probe(ref mark, ref present)) surface.Present(etchels);
                frame++;

                elapsed = Stopwatch.GetElapsedTime(origin, mark);
            }

            TimeSpan remaining = target > elapsed ? target - elapsed : TimeSpan.Zero;
            using (new Probe(ref mark, ref wait)) surface.Wait(remaining);
        }
    }
}