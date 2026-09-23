namespace Etch.Common;

public class Matrix<T>(int width, int height) : IEquatable<Matrix<T>> where T : IEquatable<T>
{
    private T[] _data = new T[width * height];
    public int Width { get; private set; } = width;
    public int Height { get; private set; } = height;
    public int Length => _data.Length;

    public Matrix(int width, int height, T fill) : 
        this(width, height) => _data.AsSpan().Fill(fill);

    public bool Equals(Matrix<T>? other)
    {
        if (other is null) return false;
        for (int i = 0; i < Length; i++)
            if (!this[i].Equals(other[i])) return false;
        return true;
    }

    public int To1D(int x, int y) => y * Width + x;
    public int To1D((int, int) position) => position switch { var (x, y) => To1D(x, y) };
    public (int X, int Y) To2D(int index) => Math.DivRem(index, Width) switch { var (y, x) => (x, y) };

    public ref T this[int index] => ref _data[index];
    public ref T this[int x, int y] => ref _data[To1D((x, y))];
    public ref T this[(int, int) position] => ref _data[To1D(position)];
    public Span<T> this[Range range] => _data.AsSpan(range);
    public Span<T> this[(int, int) position, int width] => _data.AsSpan(To1D(position), width);

    public void Swap(Matrix<T> other)
    {
        (this._data, other._data) = (other._data, this._data);
        (this.Width, other.Width) = (other.Width, this.Width);
        (this.Height, other.Height) = (other.Height, this.Height);
    }

    public readonly struct Delta(int index, T update)
    {
        public readonly int Index = index;
        public readonly T Update = update;
    }

    public int Diff(Matrix<T> other, Span<Delta> deltas)
    {
        int count = 0;
        for (int i = 0; i < Length; i++)
            if (!this[i].Equals(other[i]))
                deltas[count++] = new Delta(i, this[i]);
        return count;
    }
}
