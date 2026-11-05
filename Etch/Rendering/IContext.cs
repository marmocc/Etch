using Etch.Common;

namespace Etch.Rendering;

public interface IContext
{
    Int2 Size { get; }
    Arena Assets { get; }
    void Plot(Int2 position, Etchel etchel);
    void Blit(Handle handle);
}