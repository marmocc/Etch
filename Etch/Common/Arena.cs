using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Common;

public sealed class Arena(int capacityInBytes)
{
    /// <summary>
    /// Represents a region inside the <c>Arena</c>.
    /// </summary>
    public readonly record struct Handle(int Offset, int Length);

    private readonly byte[] _buffer = new byte[capacityInBytes];
    private int _offset = 0;

    /// <summary>
    /// Provides a way to build an <c>Arena</c> with a <paramref name="capacity"/> expressed in <typeparamref name="T"/>.
    /// </summary>
    public static Arena From<T>(int capacity) where T : unmanaged => new(capacity * Unsafe.SizeOf<T>());

    /// <summary>
    /// Provides a way to build an <c>Arena</c> from an area of size <paramref name="area"/> expressed in <typeparamref name="T"/>.
    /// </summary>
    public static Arena From<T>(Int2 area) where T : unmanaged => From<T>(area.Area());

    /// <summary>
    /// Capacity in bytes of the Arena
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// Resets the internal offset without clearing data.
    /// </summary>
    public void Reset() => _offset = 0;

    /// <summary>
    /// Clears the internal buffer and resets the offset as well.
    /// </summary>
    public void Clear() { Array.Clear(_buffer, 0, _offset); Reset(); }

    /// <summary>
    /// Accesses a region in the <c>Arena</c> from its <paramref name="handle"/>.
    /// </summary>
    public Span<byte> Access(Handle handle) => _buffer.AsSpan(handle.Offset, handle.Length);

    /// <summary>
    /// Reserves a region in the <c>Arena</c> given the <paramref name="count"/> of bytes.
    /// </summary>
    public Handle Reserve(int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(_offset + count, Capacity);
        Handle handle = new(_offset, count);
        _offset += count;
        return handle;
    }

    /// <summary>
    /// Accesses a region in the <c>Arena</c> from its <paramref name="handle"/> and treats it as a region of <typeparamref name="T"/> elements.
    /// </summary>
    public Span<T> Access<T>(Handle handle) where T : unmanaged => MemoryMarshal.Cast<byte, T>(Access(handle));

    /// <summary>
    /// Reserves a region in the <c>Arena</c> given the <paramref name="count"/> of <typeparamref name="T"/> elements.
    /// </summary>
    public Handle Reserve<T>(int count) where T : unmanaged => Reserve(count * Unsafe.SizeOf<T>());
}