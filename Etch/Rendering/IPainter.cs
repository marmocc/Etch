using Etch.Common;

namespace Etch.Rendering;

public interface IPainter
{
    void Paint(Int2 size, Batch<Packet> packets, Arena assets);
}
