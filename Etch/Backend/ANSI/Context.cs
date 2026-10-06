using Etch.Common;

namespace Etch.Backend.ANSI;

public readonly ref struct Context(long frame, double elapsed, float delta, Int2 size, Writer writer, Span<Color> back) : IContext
{
    public static ReadOnlySpan<byte> Ramp => " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
    public static byte Density(Color color) => Ramp[color.Luminance * Ramp.Length >> 8];

    private readonly Writer _writer = writer;
    private readonly Span<Color> _back = back;

    public long Frame { get; } = frame;
    public double Elapsed { get; } = elapsed;
    public float Delta { get; } = delta;

    public Int2 Size { get; } = size;
    public void Plot(Int2 position, Color color)
    {
        if (!position.AllGreaterOrEqual(Int2.Zero)) return;
        if (!position.AllLess(Size)) return;

        Flat flat = position.Flatten(Size);
        if(_back.IsEmpty || _back[flat.Value] == color) return;

        _writer.Move(position);
        _writer.Foreground(color);
        _writer.Write(Density(color));
        
        if(_back.IsEmpty) return;
        _back[flat.Value] = color;
    }

    public void Clear() { _writer.Clear(); _back.Clear(); }
}
