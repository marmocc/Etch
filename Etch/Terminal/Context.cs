using Etch.Common;

namespace Etch.Terminal;

public readonly ref struct Context(float deltaTime, Writer writer, Rect bounds)
{
    public readonly float DeltaTime = deltaTime;
    private readonly Writer _writer = writer;
    private readonly Rect _bounds = bounds;

    public bool Plot(Int2 position, byte glyph, Color foreground, Color background)
    {
        if (!_bounds.Contains(position)) return false;
        
        _writer.Move(position);
        _writer.Foreground(foreground);
        _writer.Background(background);

        _writer.GetSpan(1)[0] = glyph;
        _writer.Advance(1);

        return true;
    }

    public Span<byte> Prepare(Int2 position, int width, Color foreground, Color background)
    {
        Int2 absolutePosition = position + _bounds.Position;
        Rect writeRegion = new(absolutePosition, new(width, 1));
        if (!_bounds.Contains(writeRegion)) return Span<byte>.Empty;

        _writer.Move(absolutePosition);
        _writer.Foreground(foreground);
        _writer.Background(background);

        return _writer.GetSpan(width);
    }

    public void Commit(int written) => _writer.Advance(written);
}