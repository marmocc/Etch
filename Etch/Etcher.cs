using Etch.Common;
using Etch.Diagnostics;
using Etch.Painters;
using Etch.Rendering;
using Etch.Rendering.ANSI;

namespace Etch;

public static class Etcher
{
    public static class Terminal
    {
        /// <summary>
        /// Runs the main loop at a <paramref name="targetFps"/>.
        /// </summary>
        public static void Run<P>(int targetFps) where P : IPainter, new(), allows ref struct
        {
            P painter = new();
            using Surface surface = new();
            Batch<Packet> packets = new(8);
            Arena assets = Arena.Create<Etchel>(surface.Size.Area());
            TimeSpan interval = TimeSpan.FromSeconds(1.0 / targetFps);

            while (true)
            {
                surface.Wait(interval);
                if (surface.Stopping) return;

                assets.Reset();
                packets.Reset();

                var context = new Recorder(surface.Size, packets, assets);
                painter.Paint(context);
                context.Play<Context>(new(assets, surface));
                surface.Present();
            }
        }

        /// <summary>
        /// Runs the main loop <paramref name="warmup"/> times and then again <paramref name="actual"/> times.
        /// <para><b>This method is supposed to be used for benchmarking.</b></para>
        /// </summary>
        /// <returns>
        /// Averages of <c>paint</c>, <c>play</c> and <c>present</c> phases, in that order.
        /// </returns>
        public static (TimeSpan, TimeSpan, TimeSpan) Run(int warmup = 100, int actual = 100)
        {
            using Surface surface = new();
            Chaos painter = new();
            Batch<Packet> packets = new(8);
            Arena assets = Arena.Create<Etchel>(surface.Size.Area());

            TimeSpan paintSum = TimeSpan.Zero;
            TimeSpan playSum = TimeSpan.Zero;
            TimeSpan presentSum = TimeSpan.Zero;

            for (int i = 0; i < warmup; i++)
            {
                assets.Reset();
                packets.Reset();

                TimeSpan paint = TimeSpan.Zero;
                TimeSpan play = TimeSpan.Zero;
                TimeSpan present = TimeSpan.Zero;

                var context = new Recorder(surface.Size, packets, assets);
                using (new Watch(ref paint)) painter.Paint(context);
                using (new Watch(ref play)) context.Play<Context>(new(assets, surface));
                using (new Watch(ref present)) surface.Present();
            }

            for (int i = 0; i < actual; i++)
            {
                assets.Reset();
                packets.Reset();

                TimeSpan paint = TimeSpan.Zero;
                TimeSpan play = TimeSpan.Zero;
                TimeSpan present = TimeSpan.Zero;

                var context = new Recorder(surface.Size, packets, assets);
                using (new Watch(ref paint)) painter.Paint(context);
                using (new Watch(ref play)) context.Play<Context>(new(assets, surface));
                using (new Watch(ref present)) surface.Present();

                paintSum += paint;
                playSum += play;
                presentSum += present;
            }

            return (paintSum / actual, playSum / actual, presentSum / actual);
        }
    }
}
