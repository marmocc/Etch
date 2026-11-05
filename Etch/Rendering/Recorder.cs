using Etch.Common;

namespace Etch.Rendering;

public readonly ref struct Recorder(Int2 size, Batch<Packet> packets, Arena assets) : IContext
{
    public Int2 Size => size;
    public Arena Assets => assets;

    public void Plot(Int2 position, Etchel etchel) => throw new NotImplementedException();
    public void Blit(Handle handle) => packets.Append(new(Command.Blit, handle));

    public void Play<TContext>(TContext immediate)
        where TContext : IContext, allows ref struct
    {
        foreach(var (command, handle) in packets.Values)
        {
            switch(command)
            {
                case Command.Blit: immediate.Blit(handle); break;
                default: throw new NotImplementedException();
            }
        }
    }
}
