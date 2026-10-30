using Etch.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Color : ISequence
{
    public const int Length = 19;

    private readonly byte _escape = 0x1B;
    private readonly byte _bracket = (byte)'[';
    private readonly byte _layer;
    private readonly byte _eight = (byte)'8';
    private readonly byte _semicolon0 = (byte)';';
    private readonly byte _two = (byte)'2';
    private readonly byte _semicolon1 = (byte)';';
    private readonly Digit3 _red;
    private readonly byte _semicolon2 = (byte)';';
    private readonly Digit3 _green;
    private readonly byte _semicolon3 = (byte)';';
    private readonly Digit3 _blue;
    private readonly byte _end = (byte)'m';

    private Color(bool toggle, RGB color)
    {
        _layer = toggle ? (byte)'3' : (byte)'4';
        _red = new(color.R);
        _green = new(color.G);
        _blue = new(color.B);
    }

    public static Color Foreground(RGB color) => new(true, color);
    public static Color Background(RGB color) => new(false, color);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        MemoryMarshal.Write(buffer[written..], in this);
        written += Length;
    }
}