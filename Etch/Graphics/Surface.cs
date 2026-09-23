using Etch.Common;
using Etch.Geometry;
using System.Buffers;

namespace Etch.Graphics;

public sealed class Surface(Vector2<int> size) : ISurface<Context>
{
    public Vector2<int> Size { get; } = size;
    private readonly Stream _stream = Console.OpenStandardOutput();
    private readonly ArrayBufferWriter<byte> _output = new(8192);
    private readonly Matrix<Color> _current = new(size.X, size.Y, Color.Transparent);
    private readonly Matrix<Color> _previous = new(size.X, size.Y, Color.Transparent);
    public Context Context => new(_current);

    public void Present()
    {
        _output.Clear();
        ANSI.Move(_output, 0, 0);

        Color? lastColor = null;
        for (int y = 0; y < Size.Y; y++)
        {
            for (int x = 0; x < Size.X; x++)
            {
                Color color = _current[x, y];

                if (lastColor is null || color != lastColor.Value)
                {
                    ANSI.Color(_output, color, true);
                    lastColor = color;
                }

                ANSI.Write(_output, color.Density);
            }

            if (y < Size.Y - 1)
                ANSI.NewLine(_output);
        }

        _stream.Write(_output.WrittenSpan);
        _current[..].Fill(Color.Transparent);
    }
}
