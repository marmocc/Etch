using Etch.Rendering;
using Etch.Common;

namespace Etch.Painters;

public sealed class Chaos
{
    public int Frame = 0;
    public void Paint(Context context)
    {
        int sizeX = context.Size.X;
        int sizeY = context.Size.Y;

        Handle handle = context.Etchels.Reserve(sizeX * sizeY);
        context.FullscreenBlits.Enqueue(new(handle));

        Span<Etchel> etchels = context.Etchels.Access(handle);
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + (long)Frame * 2246822519);
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

        Frame++;
    }
}