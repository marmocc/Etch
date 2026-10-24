using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Etch.Commands;

public sealed class Batch(int capacity)
{
    [StructLayout(LayoutKind.Sequential)]
    private readonly ref struct Header(ushort command, int size)
    {
        public readonly ushort Command = command;
        public readonly ushort Reserved = 0;
        public readonly int Size = size;
    }

    private readonly ulong[] _buffer = new ulong[capacity];
    private int _count = 0;

    public int Capacity => _buffer.Length;
    public int Count => _count;
    public void Reset() => _count = 0;
    public Span<ulong> Append(ushort command, int size)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(size);
        ArgumentOutOfRangeException.ThrowIfEqual(command, 0);
        if (size > Capacity - _count - 1) throw new InvalidOperationException();

        Header header = new(command, size);
        _buffer[_count++] = Unsafe.BitCast<Header, ulong>(header);
        Span<ulong> payload = _buffer.AsSpan(_count, size);
        _count += size;
        return payload;
    }

    public Enumerator GetEnumerator() => new(_buffer.AsSpan(0, _count));
    public readonly ref struct Entry(ushort command, ReadOnlySpan<ulong> payload)
    {
        public readonly ushort Command = command;
        public readonly ReadOnlySpan<ulong> Payload = payload;
    }
    public ref struct Enumerator(ReadOnlySpan<ulong> stream)
    {
        private readonly ReadOnlySpan<ulong> _stream = stream;
        private int _position;
        private Entry _current;

        public readonly Entry Current => _current;
        public bool MoveNext()
        {
            if (_position >= _stream.Length) return false;

            Header header = Unsafe.BitCast<ulong, Header>(_stream[_position++]);
            _current = new(header.Command, _stream.Slice(_position, header.Size));
            _position += header.Size;
            return true;
        }
    }
}