namespace Etch.Backend;

public sealed class Arena<T>(int capacity) where T : unmanaged
{
    private readonly T[] _buffer = new T[capacity];
    private int _offset = 0;

    public void Reset() => _offset = 0;
    public Span<T> Allocate(int count = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_offset + count, _buffer.Length);
        Span<T> span = _buffer.AsSpan(_offset, count);
        _offset += count;
        return span;
    }
}
