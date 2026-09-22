using Etch.Buffers;
using Etch.Geometry;
using Etch.Graphics.Terminal;
using System.Buffers;

namespace Etch.Graphics.Grid;

public sealed class Surface(Vector2<int> size) : ISurface<Context>
{
    private readonly Matrix<Cell> _matrix = new(size.X, size.Y, Cell.Empty);
    private readonly ArrayBufferWriter<byte> _buffer = new(8192);
    public Vector2<int> Size { get; } = size;
    public Context Context => new(_matrix);
    public void Present()
    {
        _buffer.Clear();
        ANSI.Move(_buffer, 0, 0);

        Color? lastColor = null;
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                Cell cell = _matrix[x, y];

                if (lastColor is null || cell.Color != lastColor.Value)
                {
                    ANSI.Color(_buffer, cell.Color, true);
                    lastColor = cell.Color;
                }

                ANSI.Write(_buffer, cell.Character);
            }

            if (y < Size.Y - 1)
                ANSI.NewLine(_buffer);
        }

        Console.OpenStandardOutput().Write(_buffer.WrittenSpan);
    }
}
