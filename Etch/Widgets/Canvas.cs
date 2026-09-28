using Etch.Common;
using Etch.Graphics;
using Etch.Terminal;

namespace Etch.Widgets;

public sealed class Canvas(Int2 size, IPainter painter) : IWidget
{
    public Int2 Size { get; } = size;
    public IPainter Painter { get; } = painter;

    private Color[] _front = new Color[size.X * size.Y];
    private Color[] _back = new Color[size.X * size.Y];
    private readonly Delta[] _deltas = new Delta[size.X * size.Y];

    public void Render(Context context)
    {
        Painter.Paint(new Brush(_front, Size, context.DeltaTime));
        int count = Delta.Compute(_front, _back, _deltas);
        foreach (var delta in _deltas.AsSpan(0, count))
            context.Plot(delta.Index.Unflatten(Size.X), delta.Color);

        (_front, _back) = (_back, _front);
        Array.Clear(_front);
    }
}