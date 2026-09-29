using Etch.Common;
using Etch.Graphics;
using Etch.Graphics.Painters;
using Etch.Terminal;

namespace Etch.Widgets;

public sealed class Canvas(Int2 size, IPainter painter) : IWidget
{
    public Int2 Size { get; } = size;
    public IPainter Painter { get; } = painter;

    private Color[] _front = new Color[size.X * size.Y];
    private Color[] _back = new Color[size.X * size.Y];
    private readonly Delta[] _deltas = new Delta[size.X * size.Y];

    public static byte GlyphFrom(Color color)
    {
        int luminance = (306 * color.R + 601 * color.G + 117 * color.B) >> 10;
        int index = (luminance * 70) >> 8;
        ReadOnlySpan<byte> ramp = " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
        return ramp[index];
    }

    public void Render(Context context)
    {
        Painter.Paint(new Brush(_front, Size, context.DeltaTime));
        int count = Delta.Compute(_front, _back, _deltas);
        foreach (var delta in _deltas.AsSpan(0, count))
            context.Plot(delta.Index.Unflatten(Size.X), GlyphFrom(delta.Color), delta.Color, Color.Black);

        (_front, _back) = (_back, _front);
        Array.Clear(_front);
    }
}