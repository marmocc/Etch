using Etch.Common;
using Etch.Graphics.Painters;
using Etch.Terminal;
using System.Diagnostics;

namespace Etch.Graphics;

public sealed class Canvas(Int2 size, Writer output, IPainter painter)
{
    public Int2 Size { get; } = size;
    public Writer Output { get; } = output;
    public IPainter Painter { get; set; } = painter;

    private Color[] _front = new Color[size.X * size.Y];
    private Color[] _back = new Color[size.X * size.Y];
    private readonly Delta[] _deltas = new Delta[size.X * size.Y];

    private readonly Stopwatch _stopwatch = new();

    public static byte GlyphFrom(Color color)
    {
        int luminance = (306 * color.R + 601 * color.G + 117 * color.B) >> 10;
        int index = (luminance * 70) >> 8;
        ReadOnlySpan<byte> ramp = " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
        return ramp[index];
    }

    public void Render()
    {
        float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;

        Painter.Paint(new Brush(_front, Size, deltaTime));
        int count = Delta.Compute(_front, _back, _deltas);

        foreach (var delta in _deltas.AsSpan(0, count))
        {
            Output.Move(delta.Index.Unflatten(Size.X));
            Output.Foreground(delta.Color);
            Span<byte> buffer = Output.GetSpan(1);
            buffer[0] = GlyphFrom(delta.Color);
            Output.Advance(1);
        }

        Output.Flush();

        (_front, _back) = (_back, _front);
        Array.Clear(_front);
    }
}