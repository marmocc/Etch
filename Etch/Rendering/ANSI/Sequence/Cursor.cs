using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Cursor(bool toggle) : ISequence
{
    public const int Length = 6;

    private readonly byte _escape = 0x1B;
    private readonly byte _bracket = (byte)'[';
    private readonly byte _question = (byte)'?';
    private readonly byte _two = (byte)'2';
    private readonly byte _five = (byte)'5';
    private readonly byte _end = toggle ? (byte)'h' : (byte)'l';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        MemoryMarshal.Write(buffer[written..], in this);
        written += Length;
    }
}