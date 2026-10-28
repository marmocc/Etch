using Etch.Common;
using Etch.Rendering;
using System.Diagnostics;

namespace Etch.Painters;

public readonly ref struct Chaos : IPainter
{
    public void Paint(Int2 size, Batch<Packet> packets, Arena assets)
    {
        long ticks = Stopwatch.GetTimestamp();

        Handle handle = assets.Reserve<Etchel>(size.Area());
        packets.Append(new(Command.FullscreenBlit, handle));
        Span<Etchel> etchels = assets.Access<Etchel>(handle);
        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + ticks * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;

                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);

                etchels[Int2.Flatten(x, y, size.X)] = Etchel.Solid(new RGB(r, g, b));
            }
        }
    }
}