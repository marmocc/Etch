using Etch.Common;

namespace Etch.Rendering;

/// <summary>
/// An object packing together a <paramref name="Command"/> and a <paramref name="Handle"/>.
/// </summary>
public readonly record struct Packet(Command Command, Arena.Handle Handle);

/// <summary>
/// Defines all possible commands in Etch.
/// </summary>
public enum Command : ushort
{
    Invalid = 0,
    FullscreenBlit,
}