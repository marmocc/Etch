namespace Etch.Backend;

public interface IPainter
{
    Pace Pace { get; }
    void Paint<TContext>(TContext context) where TContext : IContext, allows ref struct;
}
