using System.Buffers;
using System.Runtime.CompilerServices;

namespace Etch.Graphics;

public sealed class Surface(int width, int height, Stream stream)
{
    private readonly Stream _stream = stream;
    private readonly ArrayBufferWriter<byte> _output = new(8192);
    private readonly Frame _current = new(width, height, Color.Transparent);
    private readonly Frame _previous = new(width, height, Color.Transparent);
    private readonly Frame.Delta[] _deltas = new Frame.Delta[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Plot(int x, int y, Color color)
    {
        if ((uint)x >= (uint)Width ||
            (uint)y >= (uint)Height) return;
        int index = y * Width + x;
        _current.Colors[index] = Color.Blend(_current.Colors[index], color);

        ReadOnlySpan<byte> ramp = " .:-=+*#%@"u8;
        int density = (8 * color.R + 26 * color.G + 3 * color.B) >> 10;
        if ((uint)density >= 10) density = 9;
        _current.Glyphs[index] = ramp[density];
    }

    public void Present()
    {
        _output.Clear();
        ANSI.Move(_output, 0, 0);

        int count = Frame.Diff(_current, _previous, _deltas);
        foreach (var delta in _deltas.AsSpan()[..count])
        {
            ANSI.Move(_output, delta.Y, delta.X);
            ANSI.Color(_output, delta.Color, true);
            ANSI.Write(_output, delta.Glyph);
        }

        _stream.Write(_output.WrittenSpan);
        Frame.Swap(_current, _previous);
        Array.Clear(_current.Colors);
        Array.Clear(_current.Glyphs);
    }
}
