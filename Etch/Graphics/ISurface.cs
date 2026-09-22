using Etch.Geometry;

namespace Etch.Graphics;

public interface ISurface<TContext> where TContext : IContext, allows ref struct
{
    Vector2<int> Size { get; }
    TContext Context { get; }
    void Present();
}
