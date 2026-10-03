using Etch.Backend;
using Etch.Common;
using Etch.Graphics.Painters;
using System.Diagnostics;

namespace Etch.Graphics;

public sealed class Canvas(Int2 size, Writer output, IPainter painter)
{
    public Int2 Size { get; } = size;
    public Writer Output { get; } = output;
    public IPainter Painter { get; set; } = painter;

    private Pixel[] _front = new Pixel[size.X * size.Y];
    private Pixel[] _back = new Pixel[size.X * size.Y];
    private readonly Delta[] _deltas = new Delta[size.X * size.Y];

    private readonly Stopwatch _stopwatch = new();

    public void Render()
    {
        float deltaTime = (float)_stopwatch.Elapsed.TotalSeconds;

        Painter.Paint(new Brush(_front, Size, deltaTime));
        int count = Delta.Compute(_front, _back, _deltas);

        foreach (var delta in _deltas.AsSpan(0, count))
        {
            Output.Move(delta.Index.Unflatten(Size.X));
            Output.Foreground(delta.Pixel.Color);
            Span<byte> buffer = Output.GetSpan(1);
            buffer[0] = delta.Pixel.Glyph;
            Output.Advance(1);
        }

        Output.Flush();

        (_front, _back) = (_back, _front);
        Array.Clear(_front);
    }
}