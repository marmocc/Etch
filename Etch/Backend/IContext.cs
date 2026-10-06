using Etch.Common;

namespace Etch.Backend;

public interface IContext
{
    long Frame { get; }
    double Elapsed { get; }
    float Delta { get; }

    Int2 Size { get; }
    void Clear();
    void Plot(Int2 position, Color color);
}