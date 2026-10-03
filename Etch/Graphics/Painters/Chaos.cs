using Etch.Common;

namespace Etch.Graphics.Painters;

public sealed class Chaos : IPainter
{
    private int _frame = 0;
    public void Paint(Brush brush)
    {
        _frame++;
        int width = brush.Available.X;
        int height = brush.Available.Y;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + _frame * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;

                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);
                Color color = new(r, g, b, 255);
                Int2 position = new(x, y);
                brush.Draw(position, new(color));
            }
        }
    }
}
