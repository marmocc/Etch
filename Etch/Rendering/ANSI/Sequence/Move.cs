using Etch.Common;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

/// <summary>
/// Represents the sequence to move the cursor.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct Move(Int2 position) : ISequence
{
    private readonly Int2 _position = position;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Span<byte> buffer, ref int written)
    {
        buffer[written++] = 0x1B;
        buffer[written++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(_position.Y + 1, buffer[written..], out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        written += rowWritten;

        buffer[written++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(_position.X + 1, buffer[written..], out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        written += colWritten;

        buffer[written++] = (byte)'H';
    }
}
