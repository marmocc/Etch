using Etch.Common;

namespace Etch.Rendering;

public readonly record struct Command(Tag Tag, Arena.Handle Handle);
public enum Tag : ushort
{
    Invalid = 0,
    FullscreenBlit,
}