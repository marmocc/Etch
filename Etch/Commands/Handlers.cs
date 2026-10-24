using Etch.Common;

namespace Etch.Commands;

public interface IHandler { void Present(Batch batch); };
public interface IFullscreenBlit : IHandler { public const ushort Tag = 1; void Handle(ReadOnlySpan<Etchel> etchels); }