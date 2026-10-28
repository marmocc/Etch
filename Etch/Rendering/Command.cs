namespace Etch.Rendering;

public readonly record struct Command(Tag Tag, ushort A, ushort B);
public enum Tag : ushort
{
    Invalid = 0,
    FullscreenBlit,
}