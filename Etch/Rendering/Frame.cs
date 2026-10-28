using Etch.Common;

namespace Etch.Rendering;

public sealed class Frame(Int2 size, int commandCapacity = 8)
{
    public readonly Arena<Etchel> Etchels = new(size.X * size.Y);
    public readonly Arena<Command> Commands = new(commandCapacity);

    public Int2 Size { get; } = size;
    public void Reset() { Etchels.Reset(); Commands.Reset(); }
    public Span<Etchel> Append(Tag tag, int length)
    {
        Handle handle = Etchels.Reserve(length);
        Commands.Add(new(tag, (ushort)handle.Offset, (ushort)handle.Length));
        return Etchels.Access(handle);
    }

    public Span<Etchel> Append() => Append(Tag.FullscreenBlit, Size.Flatten());
}