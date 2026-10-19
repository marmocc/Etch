using Etch.Common;
using Etch.Diagnostics;
using System.Diagnostics;

namespace Etch.Backend;

public static class Renderer
{
    public static void Start<TPainter>(ISurface surface, TPainter painter, TimeSpan interval) where TPainter : IPainter, allows ref struct
    {
        Etchel[] etchels = new Etchel[surface.Size.X * surface.Size.Y];
        ulong frame = 0;

        long origin = Stopwatch.GetTimestamp();
        long mark = Stopwatch.GetTimestamp();
        TimeSpan paint = TimeSpan.Zero;
        TimeSpan present = TimeSpan.Zero;
        TimeSpan wait = TimeSpan.Zero;

        while(!surface.Stopping)
        {
            TimeSpan delta = paint + present + wait;
            TimeSpan elapsed = Stopwatch.GetElapsedTime(origin);
            Context context = new(frame, elapsed, delta, surface.Size, etchels);
            using (new Probe(ref mark, ref paint)) painter.Paint(context);
            using (new Probe(ref mark, ref present)) surface.Present(etchels);
            using (new Probe(ref mark, ref wait)) surface.Wait(interval);
            frame++;
        }
    }
}