namespace Etch.Common;

/// <summary>
/// Batch is a list that only allows Append, Reset or reading all values.
/// </summary>
public sealed class Batch<T>(int capacity)
{
    private readonly T[] _buffer = new T[capacity];
    private int _offset = 0;

    public int Capacity => _buffer.Length;

    /// <summary>
    /// A <c>ReadOnlySpan</c> that points to all appended values.
    /// </summary>
    public ReadOnlySpan<T> Values => _buffer.AsSpan(0, _offset);

    /// <summary>
    /// Resets the internal offset without clearing data.
    /// </summary>
    public void Reset() => _offset = 0;

    /// <summary>
    /// Clears the internal buffer and resets the offset as well.
    /// </summary>
    public void Clear() { Array.Clear(_buffer, 0, _offset); Reset(); }

    /// <summary>
    /// Appends a value to the <c>Batch</c>.
    /// </summary>
    public void Append(T value)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_offset + 1, Capacity);
        _buffer[_offset++] = value;
    }
}