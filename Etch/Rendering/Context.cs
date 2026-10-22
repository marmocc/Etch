using Etch.Common;

namespace Etch.Rendering;

public sealed class Context(Int2 size, int capacity = 8192)
{
    public Int2 Size { get; private set; } = size;
    public readonly Arena<Etchel> Etchels = new(capacity);
    public readonly Queue<FullscreenBlit> FullscreenBlits = new(capacity);
    public readonly Queue<Blit> Blits = new(capacity);
    public readonly Queue<Plot> Plots = new(capacity);

    public void Clear()
    {
        Etchels.Reset();
        FullscreenBlits.Clear();
        Blits.Clear();
        Plots.Clear();
    }
}