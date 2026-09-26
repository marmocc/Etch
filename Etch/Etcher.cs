using Etch.Common;
using Etch.Terminal;

namespace Etch;

public sealed class Etcher(Int2 size)
{
    private readonly List<IWidget> _widgets = [];
    private readonly Writer _writer = new(size.X * size.Y * 64);

    public readonly Int2 Size = size;

    public void Add(IWidget widget) => _widgets.Add(widget);
    public void Render(Stream stream)
    {
        foreach (var widget in _widgets)
            widget.Render(new(_writer, new(Int2.Zero, Size)));
        _writer.Flush(stream);
    }
}
