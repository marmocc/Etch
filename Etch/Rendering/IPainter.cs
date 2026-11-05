namespace Etch.Rendering;

public interface IPainter
{
    void Paint<TContext>(TContext context) 
        where TContext : IContext, allows ref struct;
}
