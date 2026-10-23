namespace Etch.Rendering;

public interface ISurface : IDisposable
{
    bool Stopping { get; }
    void Present(Context context);
    void Flush();
}
