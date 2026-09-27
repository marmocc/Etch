using Etch.Common;
using Etch.Terminal;

namespace Etch.Graphics;

public readonly ref struct Context(Writer writer, Rect bounds)
{
    private readonly Writer _writer = writer;
    public readonly Rect Bounds = bounds;

    public void Write(Int2 position, Color color, byte glyph)
    {
        if (!Bounds.Contains(position)) return;
        _writer.Move(position);
        _writer.Foreground(color);
        _writer.Write(glyph);
    }

    public void Write(Int2 position, Color color, ReadOnlySpan<byte> glyphs)
    {
        Rect writeRegion = new(position, new(glyphs.Length, 1));
        if (!Bounds.Contains(writeRegion)) return;
        _writer.Move(position);
        _writer.Foreground(color);
        _writer.Write(glyphs);
    }
}