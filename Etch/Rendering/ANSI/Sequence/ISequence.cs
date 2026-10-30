using System.Runtime.CompilerServices;

namespace Etch.Rendering.ANSI.Sequence;

public interface ISequence
{
    /// <summary>
    /// Writes the sequence in <paramref name="buffer"/> and updates the <paramref name="written"/> count.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Write(Span<byte> buffer, ref int written);
}
