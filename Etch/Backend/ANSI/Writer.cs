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

        Clear();
        Write(Int2.Zero, Etchel.Empty);
    }

    public void Clear()
    {
        _buffer[_pointer++] = 0x1B;
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)'J';
    }

    public void Write(Int2 position, Etchel etchel)
    {
        if (position != _cursor)
        {
            Int2 correctedPosition = position + 1;
            _buffer[_pointer++] = 0x1B;
            _buffer[_pointer++] = (byte)'[';
            Utf8Formatter.TryFormat(correctedPosition.Y, _buffer.AsSpan(_pointer), out int rowWritten);
            _pointer += rowWritten;
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(correctedPosition.X, _buffer.AsSpan(_pointer), out int colWritten);
            _pointer += colWritten;
            _buffer[_pointer++] = (byte)'H';
            _cursor = position;
        }

        RGB foreground = etchel.Foreground;
        if (foreground != _foreground)
        {
            _buffer[_pointer++] = 0x1B;
            _buffer[_pointer++] = (byte)'[';
            _buffer[_pointer++] = (byte)'3';
            _buffer[_pointer++] = (byte)'8';
            _buffer[_pointer++] = (byte)';';
            _buffer[_pointer++] = (byte)'2';
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(foreground.R, _buffer.AsSpan(_pointer), out int redWritten);
            _pointer += redWritten;
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(foreground.G, _buffer.AsSpan(_pointer), out int greenWritten);
            _pointer += greenWritten;
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(foreground.B, _buffer.AsSpan(_pointer), out int blueWritten);
            _pointer += blueWritten;
            _buffer[_pointer++] = (byte)'m';
            _foreground = foreground;
        }

        RGB background = etchel.Background;
        if (background != _background)
        {
            _buffer[_pointer++] = 0x1B;
            _buffer[_pointer++] = (byte)'[';
            _buffer[_pointer++] = (byte)'4';
            _buffer[_pointer++] = (byte)'8';
            _buffer[_pointer++] = (byte)';';
            _buffer[_pointer++] = (byte)'2';
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(background.R, _buffer.AsSpan(_pointer), out int redWritten);
            _pointer += redWritten;
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(background.G, _buffer.AsSpan(_pointer), out int greenWritten);
            _pointer += greenWritten;
            _buffer[_pointer++] = (byte)';';
            Utf8Formatter.TryFormat(background.B, _buffer.AsSpan(_pointer), out int blueWritten);
            _pointer += blueWritten;
            _buffer[_pointer++] = (byte)'m';
            _background = background;
        }

        _buffer[_pointer++] = etchel.Glyph.Codepoint;
    }

    public void Flush()
    {
        if (_pointer == 0) return;
        _output.Write(_buffer, 0, _pointer);
        _pointer = 0;
    }
}
