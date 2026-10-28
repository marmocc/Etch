using Etch.Common;
using Etch.Rendering;
using System.Diagnostics;

namespace Etch.Painters;

public readonly ref struct Chaos : IPainter
{
    public void Paint(Frame frame)
    {
        int sizeX = frame.Size.X;
        int sizeY = frame.Size.Y;
        long ticks = Stopwatch.GetTimestamp();

        Span<Etchel> etchels = frame.Append();
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

                etchels[Int2.Flatten(x, y, sizeX)] = Etchel.Solid(new RGB(r, g, b));
            }
        }
    }
}