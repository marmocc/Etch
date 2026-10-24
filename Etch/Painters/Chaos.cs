using Etch.Commands;
using Etch.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Etch.Painters;

public readonly ref struct Chaos(int sizeX, int sizeY) : IPainter
{
    public void Paint(Batch batch)
    {
        Span<ulong> ulongs = batch.Append(IFullscreenBlit.Tag, sizeX * sizeY);
        Span<Etchel> etchels = MemoryMarshal.Cast<ulong, Etchel>(ulongs);

        long ticks = Stopwatch.GetTimestamp();
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + ticks * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;

                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);

                RGB color = new(r, g, b);
                Int2 position = new(x, y);

                int index = position.Flatten(sizeX);
                etchels[index] = Etchel.Solid(color);
            }
        }
    }
}