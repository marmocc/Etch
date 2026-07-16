namespace Etch;

public sealed class Cleanup(Action dispose) : IDisposable
{
    private readonly Action _dispose = dispose;
    public void Dispose() => _dispose();
}
