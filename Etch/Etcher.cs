using Etch.Diagnostics;
using Etch.Painters;
using Etch.Rendering;

namespace Etch;

public static class Etcher
{
    /// <summary>
    /// Runs the main loop at a <paramref name="targetFps"/>.
    /// </summary>
    public static void Run<S, P>(int targetFps)
        where S : ISurface, new() where P : IPainter, new(), allows ref struct
    {
        P painter = new();
        using S surface = new();
        Context context = new(surface.Size);
        TimeSpan interval = TimeSpan.FromSeconds(1.0 / targetFps);
        while (true)
        {
            surface.Wait(interval);
            if (surface.Stopping) return;

            painter.Paint(context);
            surface.Present(context);
            surface.Flush();
            context.Clear();
        }
    }

    /// <summary>
    /// Runs the main loop <paramref name="warmup"/> times and then again <paramref name="actual"/> times.
    /// <para><b>This method is supposed to be used for benchmarking.</b></para>
    /// </summary>
    /// <returns>
    /// Averages of <c>paint</c>, <c>present</c> and <c>flush</c> phases, in that order.
    /// </returns>
    public static (TimeSpan, TimeSpan, TimeSpan) Run<S, P>(int warmup = 100, int actual = 100)
        where S : ISurface, new() where P : IPainter, new(), allows ref struct
    {
        P painter = new();
        using S surface = new();
        Context context = new(surface.Size);

        TimeSpan paintSum = TimeSpan.Zero;
        TimeSpan presentSum = TimeSpan.Zero;
        TimeSpan flushSum = TimeSpan.Zero;

        for (int i = 0; i < warmup; i++)
        {
            TimeSpan paint = TimeSpan.Zero;
            TimeSpan present = TimeSpan.Zero;
            TimeSpan flush = TimeSpan.Zero;

            using (new Watch(ref paint)) painter.Paint(context);
            using (new Watch(ref present)) surface.Present(context);
            using (new Watch(ref flush)) surface.Flush();
            context.Clear();
        }

        for (int i = 0; i < actual; i++)
        {
            TimeSpan paint = TimeSpan.Zero;
            TimeSpan present = TimeSpan.Zero;
            TimeSpan flush = TimeSpan.Zero;

            using (new Watch(ref paint)) painter.Paint(context);
            using (new Watch(ref present)) surface.Present(context);
            using (new Watch(ref flush)) surface.Flush();
            context.Clear();

            paintSum += paint;
            presentSum += present;
            flushSum += flush;
        }

        return (paintSum / actual, presentSum / actual, flushSum / actual);
    }
}