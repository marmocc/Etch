namespace Etch.Common;

public readonly record struct Handle(int Offset, int Length);
public sealed class Arena<T>(int capacity) where T : unmanaged
{
    private readonly T[] _buffer = new T[capacity];
    private int _offset = 0;

    public int Capacity => _buffer.Length;
    public void Reset() => _offset = 0;
    public Span<T> Access(Handle handle) => _buffer.AsSpan(handle.Offset, handle.Length);
    public Handle Reserve(int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(length);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_offset + length, Capacity);
        Handle handle = new(_offset, length);
        _offset += length;
        return handle;
    }
}