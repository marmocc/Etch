using Etch.Common;
using Etch.Graphics;

namespace Etch.Backend.Ansi;

public class Surface(Int2 size, Stream stream) : ISurface
{
    public static ReadOnlySpan<byte> Ramp => " .'`^\",:_;-~!><+il?I][}{1)(|\\/tfjrxnuvczXYUJCLQ0OZmwqpdbkhao*#MW&8%B@$"u8;
    public static byte Density(Color color) => Ramp[color.Luminance * Ramp.Length >> 8];
    public readonly Writer Writer = new(8192, stream);

    public static Surface Default => field ??= new(new Int2(Console.WindowWidth, Console.WindowHeight), Console.OpenStandardOutput());

    public Color[] Last = new Color[size.X * size.Y];
    public readonly Delta[] Deltas = new Delta[size.X * size.Y];

    public void Present(Canvas canvas)
    {
        int count = Delta.Compute(Last, canvas.Data, Deltas);
        foreach(var delta in Deltas.AsSpan(0, count))
        {
            Writer.Move(delta.Index.Unflatten(size.X));
            Writer.Foreground(delta.Color);
            Writer.Write(Density(delta.Color));
        }

        canvas.Swap(ref Last);
        canvas.Clear();

        Writer.Flush();
    }
}