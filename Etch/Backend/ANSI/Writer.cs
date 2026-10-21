using Etch.Common;
using System.Buffers.Text;

namespace Etch.Backend.ANSI;

public sealed class Writer
{
    private readonly Stream _output;
    private readonly byte[] _buffer;
    private uint _pointer;

    private Int2 _cursor;
    private RGB _foreground;
    private RGB _background;

    public Writer(int capacity, Stream output) : this(capacity, output, Int2.Zero, RGB.White, RGB.Black) { }
    public Writer(int capacity, Stream output, Int2 initialCursor, RGB initialForeground, RGB initialBackground)
    {
        _output = output;
        _buffer = new byte[capacity];
        _pointer = 0;

        InternalClear();
        InternalMove(initialCursor);
        _cursor = initialCursor;
        InternalForeground(initialForeground);
        _foreground = initialForeground;
        InternalBackground(initialBackground);
        _background = initialBackground;
    }

    private void InternalMove(Int2 position)
    {
        Int2 correctedPosition = position + 1;

        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(correctedPosition.Y, _buffer.AsSpan(_pointer), out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        _pointer += (uint)rowWritten;

        _buffer[_pointer++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(correctedPosition.X, _buffer.AsSpan(_pointer), out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        _pointer += (uint)colWritten;

        _buffer[_pointer++] = (byte)'H';
    }

    private void InternalForeground(RGB foreground)
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'3'; // 3 for Foreground
        _buffer[_pointer++] = (byte)'8';
        _buffer[_pointer++] = (byte)';';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)';';

        bool rSuccess = Utf8Formatter.TryFormat(foreground.R, _buffer.AsSpan(_pointer), out int rWritten);
        if (!rSuccess) throw new InvalidOperationException("Failed to format red value.");
        _pointer += (uint)rWritten;

        _buffer[_pointer++] = (byte)';';

        bool gSuccess = Utf8Formatter.TryFormat(foreground.G, _buffer.AsSpan(_pointer), out int gWritten);
        if (!gSuccess) throw new InvalidOperationException("Failed to format green value.");
        _pointer += (uint)gWritten;

        _buffer[_pointer++] = (byte)';';

        bool bSuccess = Utf8Formatter.TryFormat(foreground.B, _buffer.AsSpan(_pointer), out int bWritten);
        if (!bSuccess) throw new InvalidOperationException("Failed to format blue value.");
        _pointer += (uint)bWritten;

        _buffer[_pointer++] = (byte)'m';
    }

    private void InternalBackground(RGB background)
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'4'; // 4 for Background
        _buffer[_pointer++] = (byte)'8';
        _buffer[_pointer++] = (byte)';';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)';';

        bool rSuccess = Utf8Formatter.TryFormat(background.R, _buffer.AsSpan(_pointer), out int rWritten);
        if (!rSuccess) throw new InvalidOperationException("Failed to format red value.");
        _pointer += (uint)rWritten;

        _buffer[_pointer++] = (byte)';';

        bool gSuccess = Utf8Formatter.TryFormat(background.G, _buffer.AsSpan(_pointer), out int gWritten);
        if (!gSuccess) throw new InvalidOperationException("Failed to format green value.");
        _pointer += (uint)gWritten;

        _buffer[_pointer++] = (byte)';';

        bool bSuccess = Utf8Formatter.TryFormat(background.B, _buffer.AsSpan(_pointer), out int bWritten);
        if (!bSuccess) throw new InvalidOperationException("Failed to format blue value.");
        _pointer += (uint)bWritten;

        _buffer[_pointer++] = (byte)'m';
    }

    private void InternalClear()
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)'J';
    }

    public void Move(Int2 position)
    {
        if (_cursor == position) return;
        InternalMove(position);
        _cursor = position;
    }

    public void Foreground(RGB foreground)
    {
        if (_foreground == foreground) return;
        InternalForeground(foreground);
        _foreground = foreground;
    }

    public void Background(RGB background)
    {
        if (_background == background) return;
        InternalBackground(background);
        _background = background;
    }

    public void Clear() => InternalClear();

    public void Write(byte value) { _buffer[_pointer++] = value; _cursor += new Int2(1, 0); }

    public void Flush()
    {
        if (_pointer == 0) return;
        _output.Write(_buffer, 0, (int)_pointer);
        _pointer = 0;
    }
}
