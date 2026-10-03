using Etch.Common;

namespace Etch.Graphics;

public static class Painters
{
    public static Color Chaos(int x, int y, int frame)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + frame * 2246822519);
        h = (h ^ (h >> 13)) * 1274126177;
        h ^= h >> 16;

        byte r = (byte)(h);
        byte g = (byte)(h >> 8);
        byte b = (byte)(h >> 16);
        Color color = new(r, g, b, 255);
        return color;
    }

    public static Color Spiral(int x, int y, int width, int height, float time)
    {
        float centerX = width / 2.0f;
        float centerY = height / 2.0f;
        float aspect = width > 0 ? (float)height / width : 1.0f;

        float dx = (x - centerX);
        float dy = (y - centerY) / aspect;

        float angle = MathF.Atan2(dy, dx);
        float radius = MathF.Sqrt(dx * dx + dy * dy);

        float n = MathF.Sin(radius * 0.5f - time * 3f + angle * 4f);
        byte v = (byte)((n + 1) / 2 * 255);

        Color color = new(v, (byte)(v / 3), (byte)(255 - v), 255);
        Int2 position = new(x, y);
        return color;
    }
}