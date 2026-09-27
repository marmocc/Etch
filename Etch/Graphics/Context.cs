using Etch.Common;
using Etch.Terminal;

namespace Etch.Graphics;

public readonly ref struct Context(Writer writer, Rect bounds)
{
    private readonly Writer _writer = writer;
    public readonly Rect Bounds = bounds;

    public void Plot(Int2 relativePosition, Color color, byte glyph)
    {
        Int2 absolutePosition = relativePosition + Bounds.Position;
        if (!Bounds.Contains(absolutePosition)) return;

        _writer.Move(absolutePosition);
        _writer.Foreground(color);
        _writer.Write(glyph);
    }

    public void Line(Int2 relativePosition, Color color, ReadOnlySpan<byte> glyphs)
    {
        Int2 absolutePosition = relativePosition + Bounds.Position;
        Rect writeRegion = new(absolutePosition, new(glyphs.Length, 1));
        if (!Bounds.Contains(writeRegion)) return;

        _writer.Move(absolutePosition);
        _writer.Foreground(color);
        _writer.Write(glyphs);
    }

    public void Blit(Int2 relativePosition, Color color, ReadOnlySpan<byte> glyphs, int width)
    {
        if (glyphs.Length % width != 0) return;

        int height = glyphs.Length / width;
        Int2 absolutePosition = relativePosition + Bounds.Position;
        Rect writeRegion = new(absolutePosition, new(width, height));
        if (!Bounds.Contains(writeRegion)) return;

        _writer.Foreground(color);
        for (int y = 0; y < height; y++)
        {
            _writer.Move(absolutePosition.AddY(y));
            _writer.Write(glyphs.Slice(y * width, width));
        }
    }
}