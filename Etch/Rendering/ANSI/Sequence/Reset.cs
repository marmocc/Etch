using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Reset() : ISequence
{
    public const int Length = 4;

    private readonly byte _escape = 0x1B;
    private readonly byte _bracket = (byte)'[';
    private readonly byte _zero = (byte)'0';
    private readonly byte _end = (byte)'m';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        MemoryMarshal.Write(buffer[written..], in this);
        written += Length;
    }
}