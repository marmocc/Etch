using Etch.Backend;
using Etch.Backend.ANSI;
using Etch.Common;

namespace Etch.Benchmarks;

internal readonly ref struct RawContext(long frame, double elapsed, float delta, Int2 size, Writer writer) : IContext
{
    public long Frame { get; } = frame;
    public double Elapsed { get; } = elapsed;
    public float Delta { get; } = delta;

    public Int2 Size { get; } = size;
    public void Plot(Int2 position, Color color)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        writer.Move(position);
        writer.Foreground(color);
        writer.Write(Surface.Density(color));
    }
}
