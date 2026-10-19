using Etch.Common;

namespace Etch.Backend;

public interface ISurface
{
    Int2 Size { get; }
    bool Stopping { get; }

    void Present(ReadOnlySpan<Etchel> etchels);
    void Wait(TimeSpan interval);

    void Wake();
    void Stop();
}