using Etch.Buffers;
using Etch.Geometry;

namespace Etch.Graphics.Grid;

public readonly ref struct Context(Matrix<Cell> cells) : IContext
{
    private readonly Matrix<Cell> _cells = cells;

    public void Clear(Color fill) => _cells[..].Fill(new Cell(fill, ' '));
    public void Plot(Vector2<int> position, Color color)
    {
        bool isInBoundsX = position.X >= 0 && position.X < _cells.Width;
        bool isInBoundsY = position.Y >= 0 && position.Y < _cells.Height;

        if (isInBoundsX && isInBoundsY)
            _cells[position] = Cell.Blend(_cells[position], new Cell(color));
    }

    public void Rectangle(Rectangle2D<int> rectangle, Color color)
    {
        int xStart = Math.Max(0, rectangle.Position.X);
        int yStart = Math.Max(0, rectangle.Position.Y);
        int xEnd = Math.Min(_cells.Width, rectangle.Position.X + rectangle.Size.X);
        int yEnd = Math.Min(_cells.Height, rectangle.Position.Y + rectangle.Size.Y);

        if (xStart >= xEnd || yStart >= yEnd) return;

        int width = xEnd - xStart;
        Cell fillCell = new(color);

        for (int y = yStart; y < yEnd; y++)
        {
            int start1D = _cells.To1D(xStart, y);
            _cells[start1D..(start1D + width)].Fill(fillCell);
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
        int maxX = Math.Min(_cells.Width - 1, bounds.Position.X + bounds.Size.X);
        int minY = Math.Max(0, bounds.Position.Y);
        int maxY = Math.Min(_cells.Height - 1, bounds.Position.Y + bounds.Size.Y);

        Cell fillCell = new(color);
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
                int start1D = _cells.To1D(scanlineStart, y);
                int length = (scanlineEnd - scanlineStart) + 1;
                _cells[start1D..(start1D + length)].Fill(fillCell);
            }
        }
    }

    public void Write(Vector2<int> position, ReadOnlySpan<char> text, Color color)
    {
        if (position.Y < 0 || position.Y >= _cells.Height || position.X >= _cells.Width) return;

        int xStart = Math.Max(0, position.X);
        int textOffset = xStart - position.X;
        int printLength = Math.Min(text.Length - textOffset, _cells.Width - xStart);

        if (printLength <= 0) return;

        int start1D = _cells.To1D(xStart, position.Y);
        Span<Cell> targetRowSlice = _cells[start1D..(start1D + printLength)];

        for (int i = 0; i < printLength; i++)
            targetRowSlice[i] = new Cell(color, text[textOffset + i]);
    }
}