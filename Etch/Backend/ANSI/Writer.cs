using Etch.Common;
using System.Buffers.Text;

namespace Etch.Backend.ANSI;

public sealed class Writer
{
    private readonly Stream _output;
    private readonly byte[] _buffer;
    private int _pointer;

    private Int2 _cursor;
    private RGB _foreground;
    private RGB _background;

    public Writer(int capacity, Stream output)
    {
        _output = output;
        _buffer = new byte[capacity];
        _pointer = 0;

        Foreground(Color.Black);
        Background(Color.Black);
        Move(Int2.Zero);
        Clear();
    }

    private void Move(Int2 position)
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        Utf8Formatter.TryFormat(position.Y + 1, _buffer.AsSpan(_pointer), out int rowWritten);
        _pointer += rowWritten;
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(position.X + 1, _buffer.AsSpan(_pointer), out int colWritten);
        _pointer += colWritten;
        _buffer[_pointer++] = (byte)'H';
        _cursor = position;
    }

    private void Foreground(RGB color)
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'3'; // 3 for Foreground
        _buffer[_pointer++] = (byte)'8';
        _buffer[_pointer++] = (byte)';';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(color.R, _buffer.AsSpan(_pointer), out int rWritten);
        _pointer += rWritten;
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(color.G, _buffer.AsSpan(_pointer), out int gWritten);
        _pointer += gWritten;
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(color.B, _buffer.AsSpan(_pointer), out int bWritten);
        _pointer += bWritten;
        _buffer[_pointer++] = (byte)'m';
        _foreground = color;
    }

    private void Background(RGB color)
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'4'; // 4 for Background
        _buffer[_pointer++] = (byte)'8';
        _buffer[_pointer++] = (byte)';';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(color.R, _buffer.AsSpan(_pointer), out int rWritten);
        _pointer += rWritten;
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(color.G, _buffer.AsSpan(_pointer), out int gWritten);
        _pointer += gWritten;
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(color.B, _buffer.AsSpan(_pointer), out int bWritten);
        _pointer += bWritten;
        _buffer[_pointer++] = (byte)'m';
        _background = color;
    }

    public void Clear()
    {
        _buffer[_pointer++] = 0x1B; // ESC
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)'J';
    }

    public void Write(Int2 position, Etchel etchel)
    {
        if (etchel.Foreground != _foreground) Foreground(etchel.Foreground);
        if (etchel.Background != _background) Background(etchel.Background);
        if (position != _cursor) Move(position);
        _buffer[_pointer++] = etchel.Glyph.Codepoint;
        _cursor += new Int2(1, 0);
    }

    public void Flush()
    {
        if (_pointer == 0) return;
        _output.Write(_buffer, 0, _pointer);
        _pointer = 0;
    }
}
