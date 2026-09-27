namespace Etch.Common;

public sealed class Ring<T>
{
    private readonly T[] _data;
    private int _head = 0;
    private int _count = 0;

    public int Count => _count;
    public int Capacity => _data.Length;

    public ReadOnlySpan<T> Head => _count < _data.Length
        ? _data.AsSpan(0, _count)
        : _data.AsSpan(_head);

    public ReadOnlySpan<T> Tail => _count < _data.Length
        ? ReadOnlySpan<T>.Empty
        : _data.AsSpan(0, _head);

    public Ring(int capacity)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "Ring capacity must be positive.");
        _data = new T[capacity];
    }

    public void Push(T value)
    {
        _data[_head++] = value;
        if (_head == _data.Length) _head = 0;
        if (_count < _data.Length) _count++;
    }
}