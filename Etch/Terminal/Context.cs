using Etch.Common;
using Etch.Logging;

namespace Etch.Terminal;

public readonly ref struct Context(Logger logger, Writer writer, Rect bounds)
{
    private readonly Writer _writer = writer;
    private readonly Rect _bounds = bounds;
    public readonly Logger Logger = logger;

    public Span<byte> Prepare(Int2 relativePosition, int width, Color foreground)
    {
        Int2 absolutePosition = relativePosition + _bounds.Position;
        Rect writeRegion = new(absolutePosition, new(width, 1));
        if (!_bounds.Contains(writeRegion)) { Logger.Log(Code.PrepareIsOutOfBounds); return Span<byte>.Empty; }

        _writer.Move(absolutePosition);
        _writer.Foreground(foreground);

        return _writer.GetSpan(width);
    }

    public void Commit(int written) => _writer.Advance(written);
}