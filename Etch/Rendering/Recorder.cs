using Etch.Common;
using Etch.Rendering.ANSI;
using System.Runtime.CompilerServices;

namespace Etch.Rendering;

public readonly ref struct Recorder(Batch<Packet> packets, Arena assets, Surface surface) : IContext
{
    public Int2 Size => surface.Size;
    public Arena Assets => assets;

    public void Plot(Int2 position, Etchel etchel) => throw new NotImplementedException();
    public void Blit(Handle handle) => packets.Append(new(Command.Blit, handle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
