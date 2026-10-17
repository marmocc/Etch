using Etch.Common;

namespace Etch.Backend;

public interface ISurface
{
    Int2 Size { get; }

    void Start<TPainter>(TPainter painter) where TPainter : IPainter, allows ref struct;
    void Wake();
    void Stop();
}