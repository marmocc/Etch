namespace Etch.Graphics;

public sealed class Frame(int width, int height)
{
    private readonly Color[] _data = new Color[width * height];

    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;
    public int Length => _data.Length;

    public Frame(int width, int height, Color fill) : 
        this(width, height) => _data.AsSpan().Fill(fill);

    public void Clear() => Array.Clear(_data);
    public void Draw(int x, int y, Color color)
    {
        int index = y * Width + x;
        Color.Blend(ref _data[index], color);
    }
    public void Draw(int x, int y, int width, Color color)
    {
        int index = y * Width + x;
        Color.Blend(_data.AsSpan(index, width), color);
    }

    public readonly record struct Delta(int X, int Y, Color Color) { }
    public static int Diff(Frame a, Frame b, Span<Delta> deltas)
    {
        int count = 0;
        for (int y = 0; y < a.Height; y++)
        {
            int rowStartIndex = y * a.Width;
            Span<Color> currentRow = a._data.AsSpan(rowStartIndex, a.Width);
            Span<Color> previousRow = b._data.AsSpan(rowStartIndex, a.Width);
            if (currentRow.SequenceEqual(previousRow)) continue;

            for (int x = 0; x < a.Width; x++)
            {
                int i = rowStartIndex + x;
                if (a._data[i].RGBA != b._data[i].RGBA)
                    deltas[count++] = new(x, y, a._data[i]);
            }
        }
        return count;
    }
}
