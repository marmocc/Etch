using Etch.Common;
using Etch.Terminal;
using Etch.Widgets;
using Etch.Diagnostics;
using System.Diagnostics;

namespace Etch;

public sealed class Etcher
{
    public readonly record struct Slot(Int2 Position, IWidget Widget) { }

    private readonly Writer _writer = new(8192);
    private readonly List<Slot> _slots = [];
    private readonly Stopwatch _stopwatch = new();
    private readonly Logger _logger = new(20);

    public void Add(Int2 position, IWidget widget) => _slots.Add(new(position, widget));
    public void Render(Stream stream)
    {
        float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;
        _logger.Log(deltaTime);
        _stopwatch.Restart();

        foreach (var slot in _slots)
            slot.Widget.Render(new(deltaTime, _writer, new(slot.Position, slot.Widget.Size)));
        _writer.Flush(stream);
    }
}