using Etch.Common;

namespace Etch.Rendering;

public sealed class Batch(Int2 size, int commandCapacity = 8)
{
    private readonly Packet[] _packets = new Packet[commandCapacity];
    private int _packetCount = 0;

    public readonly Arena Assets = Arena.From<Etchel>(size);
    public ReadOnlySpan<Packet> Packets => _packets.AsSpan(0, _packetCount);

    public Int2 Size { get; } = size;
    public void Reset() { Assets.Reset(); _packetCount = 0; }
    public Span<Etchel> Append(Command command, int length)
    {
        Arena.Handle handle = Assets.Reserve<Etchel>(length);
        _packets[_packetCount++] = new(command, handle);
        return Assets.Access<Etchel>(handle);
    }

    public Span<Etchel> Append() => Append(Command.FullscreenBlit, Size.Area());
}