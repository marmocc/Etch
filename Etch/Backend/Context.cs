using Etch.Common;

namespace Etch.Backend;

public readonly ref struct Context(ulong frame, TimeSpan elapsed, TimeSpan delta, Int2 size, Span<Etchel> etchels) : IContext
{
    private readonly Span<Etchel> _etchels = etchels;

    public ulong Frame { get; } = frame;
    public TimeSpan Elapsed { get; } = elapsed;
    public TimeSpan Delta { get; } = delta;

    public Int2 Size { get; } = size;

    public void Clear() => _etchels.Clear();
    public void Fill(Etchel etchel) => _etchels.Fill(etchel);
    public void Plot(Int2 position, Etchel etchel)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        _etchels[flat.Value] = etchel;
    }
}
