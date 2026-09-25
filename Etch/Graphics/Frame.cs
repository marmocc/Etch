using System.Runtime.InteropServices;

namespace Etch.Graphics;

public sealed class Frame(int width, int height)
{
    public Color[] Data = new Color[width * height];

    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;
    public int Length => Data.Length;

    public Frame(int width, int height, Color fill) : 
        this(width, height) => Data.AsSpan().Fill(fill);

    public void Clear() => Array.Clear(Data);

    public readonly record struct Delta(int X, int Y, Color Color) { }
    public static int Diff(Frame a, Frame b, Span<Delta> deltas)
    {
        int count = 0;
        for (int y = 0; y < a.Height; y++)
        {
            int rowStartIndex = y * a.Width;
            Span<Color> currentRow = a.Data.AsSpan(rowStartIndex, a.Width);
            Span<Color> previousRow = b.Data.AsSpan(rowStartIndex, a.Width);
            if (currentRow.SequenceEqual(previousRow)) continue;

            for (int x = 0; x < a.Width; x++)
            {
                int i = rowStartIndex + x;
                if (a.Data[i].RGBA != b.Data[i].RGBA)
                    deltas[count++] = new(x, y, a.Data[i]);
            }
        }
        return count;
    }
}
