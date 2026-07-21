using Etch.Deferred;
using Etch.Platform;

namespace Etch.UI;

public class Widget : IWidget
{
    public IReadOnlyProperty<Float2> Position => throw new NotImplementedException();
    public IReadOnlyProperty<Float2> Size => throw new NotImplementedException();
    public IReadOnlyProperty<int> ZIndex => throw new NotImplementedException();

    public event Action? Invalidated;

    public void Render<TContext>(TContext context) where TContext : IContext, allows ref struct
    {
        throw new NotImplementedException();
    }
}
