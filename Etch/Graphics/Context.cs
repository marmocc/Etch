using Etch.Geometry;

namespace Etch.Graphics;

public readonly ref struct Context(Frame frame) : IContext
{
    public void Plot(Vector2<int> position, Color color)
    {
        bool isInBoundsX = position.X >= 0 && position.X < frame.Width;
        bool isInBoundsY = position.Y >= 0 && position.Y < frame.Height;
        if (!(isInBoundsX && isInBoundsY)) return;

        frame[position] = Color.Blend(frame[position], color);
    }

    public void Rectangle(Rectangle2D<int> rectangle, Color color)
    {
        int xStart = Math.Max(0, rectangle.Position.X);
        int yStart = Math.Max(0, rectangle.Position.Y);
        int xEnd = Math.Min(frame.Width, rectangle.Position.X + rectangle.Size.X);
        int yEnd = Math.Min(frame.Height, rectangle.Position.Y + rectangle.Size.Y);
        if (xStart >= xEnd || yStart >= yEnd) return;

        int width = xEnd - xStart;

        for (int y = yStart; y < yEnd; y++)
        {
            int start1D = frame.To1D(xStart, y);
            frame[start1D..(start1D + width)].Fill(color);
        }
    }

    public void Segment(Segment2D<int> segment, Color color)
    {
        // Bresenham's Line Algorithm
        int x0 = segment.Start.X;
        int y0 = segment.Start.Y;
        int x1 = segment.End.X;
        int y1 = segment.End.Y;

        int dx = Math.Abs(x1 - x0);
        int dy = -Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int error = dx + dy;

        while (true)
        {
            Plot(new Vector2<int>(x0, y0), color);
            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * error;
            if (e2 >= dy) { error += dy; x0 += sx; }
            if (e2 <= dx) { error += dx; y0 += sy; }
        }
    }

    public void Triangle(Triangle2D<int> triangle, Color color)
    {
        var bounds = triangle.Bounds;
        int minX = Math.Max(0, bounds.Position.X);
        int maxX = Math.Min(frame.Width - 1, bounds.Position.X + bounds.Size.X);
        int minY = Math.Max(0, bounds.Position.Y);
        int maxY = Math.Min(frame.Height - 1, bounds.Position.Y + bounds.Size.Y);

        for(int y = minY; y <= maxY; y++)
        {
            int scanlineStart = -1;
            int scanlineEnd = -1;

            for (int x = minX; x <= maxX; x++)
            {
                var point = new Vector2<int>(x, y);
                if (triangle.Contains(point))
                {
                    if (scanlineStart == -1) scanlineStart = x;
                    scanlineEnd = x;
                }
            }

            if (scanlineStart != -1)
            {
                int start1D = frame.To1D(scanlineStart, y);
                int length = (scanlineEnd - scanlineStart) + 1;
                frame[start1D..(start1D + length)].Fill(color);
            }
        }
    }
}