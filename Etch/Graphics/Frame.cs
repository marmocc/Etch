namespace Etch.Graphics;

public class Frame(int width, int height) : IEquatable<Frame>
{
    private Color[] _data = new Color[width * height];
    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;
    public int Length => _data.Length;

    public Frame(int width, int height, Color fill) : 
        this(width, height) => _data.AsSpan().Fill(fill);

    public bool Equals(Frame? other)
    {
        if (other is null) return false;
        for (int i = 0; i < Length; i++)
            if (!this[i].Equals(other[i])) return false;
        return true;
    }

    public int To1D(int x, int y) => y * Width + x;
    public int To1D((int, int) position) => position switch { var (x, y) => To1D(x, y) };
    public (int X, int Y) To2D(int index) => Math.DivRem(index, Width) switch { var (y, x) => (x, y) };

    public ref Color this[int index] => ref _data[index];
    public ref Color this[int x, int y] => ref _data[To1D((x, y))];
    public ref Color this[(int, int) position] => ref _data[To1D(position)];
    public Span<Color> this[Range range] => _data.AsSpan(range);
    public Span<Color> this[(int, int) position, int width] => _data.AsSpan(To1D(position), width);

    public readonly struct Delta(int index, Color update)
    {
        public readonly int Index = index;
        public readonly Color Update = update;
    }

    public int Diff(Frame other, Span<Delta> deltas)
    {
        int count = 0;
        for (int i = 0; i < Length; i++)
            if (!this[i].Equals(other[i]))
                deltas[count++] = new Delta(i, this[i]);
        return count;
    }

    public void Swap(Frame other)
    {
        (this._data, other._data) = (other._data, this._data);
        (this.Width, other.Width) = (other.Width, this.Width);
        (this.Height, other.Height) = (other.Height, this.Height);
    }
}
