namespace Etch.Rendering.ANSI.Sequence;

public interface ISequence
{
    void Write(Span<byte> buffer, ref int written);
}
