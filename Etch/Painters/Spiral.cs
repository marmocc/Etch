using Etch.Common;
using Etch.Rendering;
using System.Diagnostics;

namespace Etch.Painters;

public readonly ref struct Spiral : IPainter
{
    public readonly long Origin = Stopwatch.GetTimestamp();
    public Spiral() { }
    public void Paint(Int2 size, Batch<Packet> packets, Arena assets)
    {
        float centerX = size.X / 2.0f;
        float centerY = size.Y / 2.0f;
        float aspect = 1f / 2f;

        TimeSpan elapsed = Stopwatch.GetElapsedTime(Origin);

        Handle handle = assets.Reserve<Etchel>(size.Area());
        packets.Append(new(Command.FullscreenBlit, handle));
        Span<Etchel> etchels = assets.Access<Etchel>(handle);
        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                float dx = (x - centerX);
                float dy = (y - centerY) / aspect;

                float angle = MathF.Atan2(dy, dx);
                float radius = MathF.Sqrt(dx * dx + dy * dy);

                float n = MathF.Sin(radius * 0.5f - (float)elapsed.TotalSeconds * 3f + angle * 4f);
                byte v = (byte)((n + 1) / 2 * 255);

                RGB color = new(v, (byte)(v / 3), (byte)(255 - v));
                etchels[Int2.Flatten(x, y, size.X)] = Etchel.Solid(color);
            }
        }
    }
}