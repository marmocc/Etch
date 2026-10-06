using Etch.Common;

namespace Etch.Backend;

public interface ISurface<TContext> where TContext : IContext, allows ref struct
{
    Int2 Size { get; }
    void Run<TPainter>(TPainter painter) where TPainter : IPainter, allows ref struct;
    void Stop();
}
