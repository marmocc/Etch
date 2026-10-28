using Etch.Common;
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
        Batch<Packet> packets = new(8);
        Arena assets = Arena.Create<Etchel>(surface.Size.Area());
        TimeSpan interval = TimeSpan.FromSeconds(1.0 / targetFps);

        while (true)
        {
            surface.Wait(interval);
            if (surface.Stopping) return;

            assets.Reset();
            packets.Reset();

            painter.Paint(surface.Size, packets, assets);
            surface.Present(packets, assets);
            surface.Flush();
        }
    }

    /// <summary>
    /// Runs the main loop <paramref name="warmup"/> times and then again <paramref name="actual"/> times.
    /// <para><b>This method is supposed to be used for benchmarking.</b></para>
    /// </summary>
    /// <returns>
    /// Averages of <c>paint</c>, <c>present</c> and <c>flush</c> phases, in that order.
    /// </returns>
    public static (TimeSpan, TimeSpan, TimeSpan) Run<S>(int warmup = 100, int actual = 100)
        where S : ISurface, new()
    {
        using S surface = new();
        Chaos painter = new();
        Batch<Packet> packets = new(8);
        Arena assets = Arena.Create<Etchel>(surface.Size.Area());

        TimeSpan paintSum = TimeSpan.Zero;
        TimeSpan presentSum = TimeSpan.Zero;
        TimeSpan flushSum = TimeSpan.Zero;

        for (int i = 0; i < warmup; i++)
        {
            assets.Reset();
            packets.Reset();

            TimeSpan paint = TimeSpan.Zero;
            TimeSpan present = TimeSpan.Zero;
            TimeSpan flush = TimeSpan.Zero;

            using (new Watch(ref paint)) painter.Paint(surface.Size, packets, assets);
            using (new Watch(ref present)) surface.Present(packets, assets);
            using (new Watch(ref flush)) surface.Flush();
        }

        for (int i = 0; i < actual; i++)
        {
            assets.Reset();
            packets.Reset();

            TimeSpan paint = TimeSpan.Zero;
            TimeSpan present = TimeSpan.Zero;
            TimeSpan flush = TimeSpan.Zero;

            using (new Watch(ref paint)) painter.Paint(surface.Size, packets, assets);
            using (new Watch(ref present)) surface.Present(packets, assets);
            using (new Watch(ref flush)) surface.Flush();

            paintSum += paint;
            presentSum += present;
            flushSum += flush;
        }

        return (paintSum / actual, presentSum / actual, flushSum / actual);
    }
}