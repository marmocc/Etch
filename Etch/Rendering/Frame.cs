using Etch.Common;

namespace Etch.Rendering;

public sealed class Frame(Int2 size, int commandCapacity = 8)
{
    private readonly Command[] _commands = new Command[commandCapacity];
    private int _commandCount = 0;

    public readonly Arena Assets = Arena.From<Etchel>(size);
    public ReadOnlySpan<Command> Commands => _commands.AsSpan(0, _commandCount);

    public Int2 Size { get; } = size;
    public void Reset() { Assets.Reset(); _commandCount = 0; }
    public Span<Etchel> Append(Tag tag, int length)
    {
        Arena.Handle handle = Assets.Reserve<Etchel>(length);
        _commands[_commandCount++] = new(tag, handle);
        return Assets.Access<Etchel>(handle);
    }

    public Span<Etchel> Append() => Append(Tag.FullscreenBlit, Size.Area());
}