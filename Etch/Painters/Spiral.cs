using Etch.Commands;
using Etch.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Etch.Painters;

public readonly ref struct Spiral(int sizeX, int sizeY) : IPainter
{
    public readonly long Origin = Stopwatch.GetTimestamp();
    public void Paint(Batch batch)
    {
        Span<ulong> ulongs = batch.Append(IFullscreenBlit.Tag, sizeX * sizeY);
        Span<Etchel> etchels = MemoryMarshal.Cast<ulong, Etchel>(ulongs);

        float centerX = sizeX / 2.0f;
        float centerY = sizeY / 2.0f;
        float aspect = 1f / 2f;

        TimeSpan elapsed = Stopwatch.GetElapsedTime(Origin);
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                float dx = (x - centerX);
                float dy = (y - centerY) / aspect;

                float angle = MathF.Atan2(dy, dx);
                float radius = MathF.Sqrt(dx * dx + dy * dy);

                float n = MathF.Sin(radius * 0.5f - (float)elapsed.TotalSeconds * 3f + angle * 4f);
                byte v = (byte)((n + 1) / 2 * 255);

                RGB color = new(v, (byte)(v / 3), (byte)(255 - v));
                Int2 position = new(x, y);

                int index = position.Flatten(sizeX);
                etchels[index] = Etchel.Solid(color);
            }
        }
    }
}