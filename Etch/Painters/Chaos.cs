using Etch.Backend;
using Etch.Common;

namespace Etch.Painters;

public readonly ref struct Chaos : IPainter
{
    public void Paint<TContext>(TContext context) where TContext : IContext, allows ref struct
    {
        for(int y = 0; y < context.Size.Y; y++)
        {
            for (int x = 0; x < context.Size.X; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + context.Frame * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;

                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);
                context.Plot(new Int2(x, y), new Color(r, g, b, 255));
            }
        }
    }
}