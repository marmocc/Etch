using Etch.Common;

namespace Etch.Backend;

public interface IContext
{
    Timings Timings { get; }
    Int2 Size { get; }
    void Clear(RGB fill);
    void Plot(Int2 position, Etchel etchel);
}