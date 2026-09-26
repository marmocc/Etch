using Etch.Common;
using Etch.Terminal;

namespace Etch.Graphics;

public readonly ref struct Context(Writer writer, Rect bounds)
{
    private readonly Writer _writer = writer;
    public readonly Rect Bounds = bounds;

    public void Write(Int2 position, Color color, byte glyph)
    {
        if (!Bounds.Interior(position)) return;
        _writer.Move(position);
        _writer.Foreground(color);
        _writer.Write(glyph);
    }
}