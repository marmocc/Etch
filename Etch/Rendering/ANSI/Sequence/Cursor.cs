using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

/// <summary>
/// Represents the sequence to show or hide the cursor.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Cursor : ISequence
{
    public const int Length = 6;

    private readonly byte _escape = 0x1B;
    private readonly byte _bracket = (byte)'[';
    private readonly byte _question = (byte)'?';
    private readonly byte _two = (byte)'2';
    private readonly byte _five = (byte)'5';
    private readonly byte _end;

    private Cursor(bool toggle) => _end = toggle ? (byte)'h' : (byte)'l';

    /// <summary>
    /// Builds the sequence to show the cursor.
    /// </summary>
    public static Cursor Show() => new(true);

    /// <summary>
    /// Builds the sequence to hide the cursor.
    /// </summary>
    public static Cursor Hide() => new(false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        MemoryMarshal.Write(buffer[written..], in this);
        written += Length;
    }
}