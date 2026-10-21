using Etch.Common;

namespace Etch.Backend.ANSI;

public readonly ref struct Context(Timings timings, Int2 size, Span<Etchel> shadow, Writer writer) : IContext
{
    private readonly Span<Etchel> _shadow = shadow;
    private readonly Writer _writer = writer;

    public Timings Timings { get; } = timings;
    public Int2 Size { get; } = size;

    public void Clear(RGB fill) { _shadow.Fill(Etchel.Solid(fill)); _writer.Clear(fill); }
    public void Plot(Int2 position, Etchel etchel)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        if (Intrinsics.Equals<Etchel, ulong>(etchel, _shadow[flat.Value])) return;
        _shadow[flat.Value] = etchel;

        _writer.Write(position, etchel);
    }
}
