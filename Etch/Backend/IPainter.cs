namespace Etch.Backend;

public interface IPainter
{
    Pace Paint<TContext>(TContext context) where TContext : IContext, allows ref struct;
}
