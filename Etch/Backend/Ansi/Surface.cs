using Etch.Common;
using Etch.Graphics;
using System.Runtime.InteropServices;

namespace Etch.Backend.Ansi;

public class Surface(Int2 size, Stream stream) : ISurface
{
    public static ReadOnlySpan<byte> Ramp => " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
    public static byte Density(Color color) => Ramp[color.Luminance * Ramp.Length >> 8];
    public readonly Writer Writer = new(8192, stream);
    public Color[] Last = new Color[size.X * size.Y];

    public static Surface Default => field ??= new(new Int2(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput());

    public void Present(Canvas canvas)
    {
        ReadOnlySpan<Color> front = canvas.Data;
        Span<Color> back = Last;

        ReadOnlySpan<uint> frontAsUint = MemoryMarshal.Cast<Color, uint>(front);
        ReadOnlySpan<uint> backAsUint = MemoryMarshal.Cast<Color, uint>(back);
        if (frontAsUint.SequenceEqual(backAsUint)) return;

        for (int i = 0; i < front.Length; i++)
        {
            Color current = front[i];
            if (current != back[i])
            {
                back[i] = current;
                Writer.Move(new Flat(i).Unflatten(size.X));
                Writer.Foreground(current);
                Writer.Write(Density(current));
            }

        }

        Writer.Flush();
    }
}