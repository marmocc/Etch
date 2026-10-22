using Etch.Common;

namespace Etch.Rendering;

public interface ICommand;
public readonly record struct Plot(short X, short Y, byte R, byte G, byte B, byte A) : ICommand;
public readonly record struct FullscreenBlit(Handle Handle) : ICommand;
public readonly record struct Blit(Int2 Position, Handle Handle) : ICommand;