using Etch.Common;
using Etch.Logging;
using Etch.Terminal;
using System.Diagnostics;

namespace Etch;

public sealed class Etcher(Int2 size)
{
    private readonly List<IWidget> _widgets = [];
    private readonly Writer _writer = new(size.X * size.Y * 64);
    private readonly Stopwatch _stopwatch = new();
    private readonly Logger _logger = new(20);

    public readonly Int2 Size = size;

    public void Add(IWidget widget) => _widgets.Add(widget);
    public void Render(Stream stream)
    {
        _stopwatch.Restart();

        foreach (var widget in _widgets)
            widget.Render(new(_logger, _writer, new(Int2.Zero, widget.Size)));
        _writer.Flush(stream);

        _stopwatch.Stop();
        _logger.Log((float)_stopwatch.Elapsed.TotalMilliseconds);
    }
}