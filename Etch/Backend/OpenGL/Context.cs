using Etch.Common;

namespace Etch.Backend.OpenGL;

public readonly ref struct Context(long frame, double elapsed, float delta, Int2 size, Span<Color> cells) : IContext
{
    private readonly Span<Color> _cells = cells;

    public long Frame { get; } = frame;
    public double Elapsed { get; } = elapsed;
    public float Delta { get; } = delta;

    public Int2 Size { get; } = size;
    public void Plot(Int2 position, Color color)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        _cells[flat.Value] = color;
    }
    public void Clear() => _cells.Clear();
}
