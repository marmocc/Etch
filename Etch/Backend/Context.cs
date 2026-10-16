using Etch.Common;

namespace Etch.Backend;

public readonly ref struct Context(long frame, double elapsed, float delta, Int2 size, Span<Etchel> etchels) : IContext
{
    private readonly Span<Etchel> _etchels = etchels;

    public long Frame { get; } = frame;
    public double Elapsed { get; } = elapsed;
    public float Delta { get; } = delta;

    public Int2 Size { get; } = size;
    public void Plot(Int2 position, Etchel etchel)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        _etchels[flat.Value] = etchel;
    }
}
