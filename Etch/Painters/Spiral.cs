using Etch.Backend;
using Etch.Common;

namespace Etch.Painters;

public readonly ref struct Spiral : IPainter
{
    public void Paint<TContext>(TContext context) where TContext : IContext, allows ref struct
    {
        float centerX = context.Size.X / 2.0f;
        float centerY = context.Size.Y / 2.0f;
        float aspect = context.Size.X > 0 ? (float)context.Size.Y / context.Size.X : 1.0f;

        for (int y = 0; y < context.Size.Y; y++)
        {
            for (int x = 0; x < context.Size.X; x++)
            {
                float dx = (x - centerX);
                float dy = (y - centerY) / aspect;

                float angle = MathF.Atan2(dy, dx);
                float radius = MathF.Sqrt(dx * dx + dy * dy);

                float n = MathF.Sin(radius * 0.5f - (float)context.Elapsed * 3f + angle * 4f);
                byte v = (byte)((n + 1) / 2 * 255);

                Color color = new(v, (byte)(v / 3), (byte)(255 - v), 255);
                Int2 position = new(x, y);
                context.Plot(position, color);
            }
        }
    }
}