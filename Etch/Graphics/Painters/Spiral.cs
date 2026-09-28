using Etch.Common;

namespace Etch.Graphics.Painters;

public sealed class Spiral : IPainter
{
    float time = 0;
    public void Paint(Brush brush)
    {
        time += brush.DeltaTime;
        var width = brush.Available.X;
        var height = brush.Available.Y;

        float centerX = width / 2.0f;
        float centerY = height / 2.0f;
        float aspect = width > 0 ? (float)height / width : 1.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float dx = (x - centerX);
                float dy = (y - centerY) / aspect;

                float angle = MathF.Atan2(dy, dx);
                float radius = MathF.Sqrt(dx * dx + dy * dy);

                float n = MathF.Sin(radius * 0.5f - time * 3f + angle * 4f);
                byte v = (byte)((n + 1) / 2 * 255);

                Color color = new(v, (byte)(v / 3), (byte)(255 - v), 255);
                Int2 position = new(x, y);
                brush.Draw(position, color);
            }
        }
    }
}
