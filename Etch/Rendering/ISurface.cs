using Etch.Common;

namespace Etch.Rendering;

public interface ISurface : IDisposable
{
    Int2 Size { get; }
    bool Stopping { get; }
    void Wait(TimeSpan interval);
    void Present(Context context);
    void Flush();

    void Stop();
    void Wake();
}
