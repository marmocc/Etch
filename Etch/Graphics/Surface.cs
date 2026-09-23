using Etch.Geometry;
using System.Buffers;

namespace Etch.Graphics;

public sealed class Surface(Vector2<int> size) : ISurface<Context>
{
    private readonly Frame _current = new(size.X, size.Y, Color.Transparent);
    private readonly Frame _previous = new(size.X, size.Y, Color.Transparent);
    private readonly Frame.Delta[] _deltas = new Frame.Delta[size.X * size.Y];
    private readonly Stream _stream = Console.OpenStandardOutput();
    private readonly ArrayBufferWriter<byte> _output = new(8192);
    
    public Vector2<int> Size { get; } = size;
    public Context Context => new(_current);

    public void Present()
    {
        _output.Clear();
        ANSI.Move(_output, 0, 0);

        int count = _current.Diff(_previous, _deltas);
        foreach (var delta in _deltas.AsSpan()[..count])
        {
            var (x, y) = _current.To2D(delta.Index);
            ANSI.Move(_output, y, x);
            ANSI.Color(_output, delta.Update, true);
            ANSI.Write(_output, delta.Update.Density);
        }

        _stream.Write(_output.WrittenSpan);
        _current.Swap(_previous);
        _current[..].Fill(Color.Transparent);
    }
}
