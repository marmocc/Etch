using Etch.Common;

namespace Etch.Graphics;

public sealed class Frame(Int2 size)
{
    private readonly Color[] _data = new Color[size.X * size.Y];
    public readonly Int2 Size = size;

    public Frame(Int2 size, Color fill) : 
        this(size) => Array.Fill(_data, fill);
    public void Clear() => Array.Clear(_data);

    private int To1D(Int2 position) => position.Y * Size.X + position.X;
    public ref Color this[Int2 position] => ref _data[To1D(position)];
    public Span<Color> this[Int2 position, int width] => _data.AsSpan(To1D(position), width);

    public readonly record struct Delta(Int2 Position, Color Color) { }
    public static int Diff(Frame a, Frame b, Span<Delta> deltas)
    {
        int count = 0;
        for (int y = 0; y < a.Size.Y; y++)
        {
            int rowStartIndex = y * a.Size.X;
            Span<Color> currentRow = a._data.AsSpan(rowStartIndex, a.Size.X);
            Span<Color> previousRow = b._data.AsSpan(rowStartIndex, a.Size.X);
            if (currentRow.SequenceEqual(previousRow)) continue;

            for (int x = 0; x < a.Size.X; x++)
            {
                int i = rowStartIndex + x;
                Color current = a._data[i];
                if (current.RGBA != b._data[i].RGBA)
                    deltas[count++] = new(new(x, y), current);
            }
        }
        return count;
    }
}
