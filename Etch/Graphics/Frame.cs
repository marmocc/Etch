using System.Runtime.InteropServices;

namespace Etch.Graphics;

public class Frame(int width, int height) : IEquatable<Frame>
{
    public Color[] Colors = new Color[width * height];
    public byte[] Glyphs = new byte[width * height];

    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;
    public int Length => Colors.Length;

    public Frame(int width, int height, Color fill) : 
        this(width, height) => Colors.AsSpan().Fill(fill);

    public bool Equals(Frame? other)
    {
        if (other is null) return false;
        return MemoryMarshal.Cast<Color, uint>(this.Colors)
            .SequenceEqual(MemoryMarshal.Cast<Color, uint>(other.Colors));
    }

    public readonly record struct Delta(int X, int Y, Color Color, byte Glyph) { }
    public static int Diff(Frame a, Frame b, Span<Delta> deltas)
    {
        int count = 0;
        for (int y = 0; y < a.Height; y++)
        {
            int rowStartIndex = y * a.Width;
            Span<Color> currentRow = a.Colors.AsSpan(rowStartIndex, a.Width);
            Span<Color> previousRow = b.Colors.AsSpan(rowStartIndex, a.Width);
            Span<uint> currentRowAsUint = MemoryMarshal.Cast<Color, uint>(currentRow);
            Span<uint> previousRowAsUint = MemoryMarshal.Cast<Color, uint>(previousRow);
            if (currentRowAsUint.SequenceEqual(previousRowAsUint)) continue;

            for (int x = 0; x < a.Width; x++)
            {
                int i = rowStartIndex + x;
                if (a.Colors[i].RGBA != b.Colors[i].RGBA)
                    deltas[count++] = new(x, y, a.Colors[i], a.Glyphs[i]);
            }
        }
        return count;
    }

    public static void Swap(Frame a, Frame b)
    {
        (a.Colors, b.Colors) = (b.Colors, a.Colors);
        (a.Glyphs, b.Glyphs) = (b.Glyphs, a.Glyphs);
        (a.Width, b.Width) = (b.Width, a.Width);
        (a.Height, b.Height) = (b.Height, a.Height);
    }
}
