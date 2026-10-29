using Etch.Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Color(bool toggle, RGB color) : ISequence
{
    public const int Length = 19;

    private readonly byte _escape = 0x1B;
    private readonly byte _bracket = (byte)'[';
    private readonly byte _layer = toggle ? (byte)'3' : (byte)'4';
    private readonly byte _eight = (byte)'8';
    private readonly byte _semicolon0 = (byte)';';
    private readonly byte _two = (byte)'2';
    private readonly byte _semicolon1 = (byte)';';
    private readonly Digits3 _red = new(color.R);
    private readonly byte _semicolon2 = (byte)';';
    private readonly Digits3 _green = new(color.G);
    private readonly byte _semicolon3 = (byte)';';
    private readonly Digits3 _blue = new(color.B);
    private readonly byte _end = (byte)'m';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        MemoryMarshal.Write(buffer[written..], in this);
        written += Length;
    }
}