using Etch.Common;

namespace Etch.Backend.OpenGL;

public readonly ref struct Context(Timings timings, Int2 size, Span<Etchel> buffer) : IContext
{
    private readonly Span<Etchel> _buffer = buffer;

    public Timings Timings { get; } = timings;
    public Int2 Size { get; } = size;

    public void Clear() => _buffer.Clear();
    public void Fill(Etchel etchel) => _buffer.Fill(etchel);

    public void Plot(Int2 position, Etchel etchel)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        _buffer[flat.Value] = etchel;
    }
}
