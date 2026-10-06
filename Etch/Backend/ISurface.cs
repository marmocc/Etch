namespace Etch.Backend;

public interface ISurface<TContext> where TContext : IContext, allows ref struct
{
    void Run(Action<TContext> draw);
    void Stop();
}
