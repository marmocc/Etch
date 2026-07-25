using Etch.Primitives;

namespace Etch.Platform;

public delegate void Renderer<TContext>(TContext context) where TContext : IContext, allows ref struct;
public interface IWindow<TContext> : IDisposable where TContext : IContext, allows ref struct
{
    Float2 Size { get; }

    event Action<Float2>? Resized;
    event Renderer<TContext>? Rendering;

    void Run();
    void Close();
    void Invalidate();
}