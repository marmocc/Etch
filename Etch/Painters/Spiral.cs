using Etch.Common;
using Etch.Rendering;
using System.Diagnostics;

namespace Etch.Painters;

public readonly ref struct Spiral : IPainter
{
    public readonly long Origin = Stopwatch.GetTimestamp();
    public Spiral() { }
    public void Paint(Frame frame)
    {
        int sizeX = frame.Size.X;
        int sizeY = frame.Size.Y;

        float centerX = sizeX / 2.0f;
        float centerY = sizeY / 2.0f;
        float aspect = 1f / 2f;

        Span<Etchel> etchels = frame.Append();
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
                etchels[Int2.Flatten(x, y, sizeX)] = Etchel.Solid(color);
            }
        }
    }
}