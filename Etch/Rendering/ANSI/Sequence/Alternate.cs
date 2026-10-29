using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Alternate(bool toggle) : ISequence
{
    public const int Length = 8;

    private readonly byte _escape = 0x1B;
    private readonly byte _bracket = (byte)'[';
    private readonly byte _question = (byte)'?';
    private readonly byte _one = (byte)'1';
    private readonly byte _zero = (byte)'0';
    private readonly byte _four = (byte)'4';
    private readonly byte _nine = (byte)'9';
    private readonly byte _end = toggle ? (byte)'h' : (byte)'l';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        MemoryMarshal.Write(buffer[written..], in this);
        written += Length;
    }
}