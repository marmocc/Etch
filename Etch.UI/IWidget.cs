namespace Etch.UI;

using Etch.Platform;
using Etch.Deferred;
using Etch.Primitives;

public interface IWidget
{
    IReadOnlyProperty<Float2> Position { get; }
    IReadOnlyProperty<Float2> Size { get; }
    IReadOnlyProperty<int> ZIndex { get; }

    event Action? Invalidated;

    void Render<TContext>(TContext context) where TContext : IContext, allows ref struct;
}
