namespace Etch.Deferred;

public sealed class Cleanup(Action dispose) : IDisposable
{
    private bool _disposed;
    private readonly Action _dispose = dispose;
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _dispose();
    }
}
