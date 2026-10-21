using Etch.Common;

namespace Etch.Backend;

public interface IContext
{
    Timings Timings { get; }
    Int2 Size { get; }
    void Clear();
    void Plot(Int2 position, Etchel etchel);
}