namespace Etch.Buffers;

public readonly struct Matrix<T>(int width, int height)
{
    private readonly T[] _data = new T[width * height];

    public int Width { get; } = width;
    public int Height { get; } = height;
    public int Length => _data.Length;

    public Matrix(int width, int height, T defaultValue) : 
        this(width, height) => Array.Fill(_data, defaultValue);

    public int To1D(int x, int y) => y * Width + x;
    public int To1D((int, int) position) => position switch { var (x, y) => To1D(x, y) };
    public (int x, int y) To2D(int index) => Math.DivRem(index, Width) switch { var (y, x) => (x, y) };

    public ref T this[int index] => ref _data[index];
    public ref T this[int x, int y] => ref _data[To1D((x, y))];
    public ref T this[(int, int) position] => ref _data[To1D(position)];
    public Span<T> this[Range range] => _data.AsSpan(range);
    public Span<T> this[(int, int) position, int width] => _data.AsSpan(To1D(position), width);
}
