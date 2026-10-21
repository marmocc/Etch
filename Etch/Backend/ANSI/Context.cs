using Etch.Common;

namespace Etch.Backend.ANSI;

public readonly ref struct Context(Timings timings, Int2 size, Span<Etchel> shadow, Writer writer) : IContext
{
    private readonly Span<Etchel> _shadow = shadow;
    private readonly Writer _writer = writer;

    public Timings Timings { get; } = timings;
    public Int2 Size { get; } = size;

    public void Clear(RGB fill) 
    { 
        _shadow.Fill(Etchel.Solid(fill));
        if(_writer.Background != fill) _writer.Color(fill, false);
        _writer.Clear(); 
    }
    public void Plot(Int2 position, Etchel etchel)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        if (Intrinsics.Equals<Etchel, ulong>(etchel, _shadow[flat.Value])) return;
        _shadow[flat.Value] = etchel;

        if (_writer.Position != position) _writer.Move(position);
        if (_writer.Foreground != etchel.Foreground) _writer.Color(etchel.Foreground, true);
        if (_writer.Background != etchel.Background) _writer.Color(etchel.Background, false);
        _writer.Write(etchel.Glyph.Codepoint);
    }
}
