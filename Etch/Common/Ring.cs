using System.Diagnostics.CodeAnalysis;

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

    public T Peek()
    {
        if(TryPeek(out var value)) return value; 
        throw new InvalidOperationException("Can't Peek into a Ring with no elements.");
    }
    public bool TryPeek([MaybeNullWhen(false)] out T value)
    {
        if (_count == 0) { value = default; return false; }
        int lastIndex = _head == 0 ? _data.Length - 1 : _head - 1;
        value = _data[lastIndex];
        return true;
    }

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