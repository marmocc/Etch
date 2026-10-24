using Etch.Commands;
using Etch.Common;

namespace Etch.Rendering;

public interface ISurface : IHandler, IDisposable
{
    Int2 Size { get; }
    bool Stopping { get; }
    void Wait(TimeSpan interval);
    void Flush();

    void Stop();
    void Wake();
}
