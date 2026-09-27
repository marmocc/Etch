namespace Etch.Common;

public sealed class Matrix<T>(Int2 size)
{
    private readonly T[] _data = new T[size.X * size.Y];
    public readonly Int2 Size = size;

    public Matrix(Int2 size, T fill) :
        this(size) => Array.Fill(_data, fill);
    public void Clear() => Array.Clear(_data);

    private int To1D(Int2 position) => position.Y * Size.X + position.X;
    public ref T this[Int2 position] => ref _data[To1D(position)];
    public Span<T> this[Int2 position, int width] => _data.AsSpan(To1D(position), width);

    public readonly record struct Delta(Int2 Position, T Value) { }
    public static int Diff(Matrix<T> a, Matrix<T> b, Span<Delta> deltas)
    {
        int count = 0;
        for (int y = 0; y < a.Size.Y; y++)
        {
            int rowStartIndex = y * a.Size.X;
            Span<T> currentRow = a._data.AsSpan(rowStartIndex, a.Size.X);
            Span<T> previousRow = b._data.AsSpan(rowStartIndex, a.Size.X);
            if (currentRow.SequenceEqual(previousRow)) continue;

            for (int x = 0; x < a.Size.X; x++)
            {
                int i = rowStartIndex + x;
                T current = a._data[i];
                T previous = b._data[i];
                if (!EqualityComparer<T>.Default.Equals(current, previous))
                    deltas[count++] = new(new(x, y), current);
            }
        }
        return count;
    }
}