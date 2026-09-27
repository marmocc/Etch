using Etch.Common;
using Etch.Logging;
using Etch.Terminal;

namespace Etch.Graphics;

public readonly ref struct Context(Logger logger, Writer writer, Rect bounds)
{
    private readonly Writer _writer = writer;
    public readonly Logger Logger = logger;
    public readonly Rect Bounds = bounds;

    public void Plot(Int2 relativePosition, Color color, byte glyph)
    {
        Int2 absolutePosition = relativePosition + Bounds.Position;
        if (!Bounds.Contains(absolutePosition)) { Logger.Log(Warning.PlotOutOfBounds); return; }

        _writer.Move(absolutePosition);
        _writer.Foreground(color);
        _writer.Write(glyph);
    }

    public void Blit(Int2 relativePosition, Color color, ReadOnlySpan<byte> glyphs)
    {
        Int2 absolutePosition = relativePosition + Bounds.Position;
        Rect writeRegion = new(absolutePosition, new(glyphs.Length, 1));
        if (!Bounds.Contains(writeRegion)) { Logger.Log(Warning.BlitOutOfBounds); return; }

        _writer.Move(absolutePosition);
        _writer.Foreground(color);
        _writer.Write(glyphs);
    }
}