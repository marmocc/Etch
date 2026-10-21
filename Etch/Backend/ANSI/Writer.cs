using Etch.Common;
using Silk.NET.GLFW;
using System.Buffers.Text;

namespace Etch.Backend.ANSI;

public sealed class Writer
{
    public const byte Escape = 0x1B;

    private readonly Stream _output;
    private readonly byte[] _buffer;
    private int _pointer;

    public Int2 Position { get; private set; }
    public RGB Foreground { get; private set; }
    public RGB Background { get; private set; }

    public Writer(int capacity, Stream output)
    {
        _output = output;
        _buffer = new byte[capacity];
        _pointer = 0;

        Color(RGB.Black, true);
        Color(RGB.Black, false);
        Move(Int2.Zero);
    }

    /// <summary>
    /// Emits <c>ESC[Y;XH</c> to set the cursor position.
    /// </summary>
    public void Move(Int2 position)
    {
        _buffer[_pointer++] = Escape;
        _buffer[_pointer++] = (byte)'[';
        Utf8Formatter.TryFormat(position.Y + 1, _buffer.AsSpan(_pointer), out int rowWritten);
        _pointer += rowWritten;
        _buffer[_pointer++] = (byte)';';
        Utf8Formatter.TryFormat(position.X + 1, _buffer.AsSpan(_pointer), out int colWritten);
        _pointer += colWritten;
        _buffer[_pointer++] = (byte)'H';
        
        Position = position;
    }

    /// <summary>
    /// Emits <c>ESC[38;2;R;G;Bm</c> to set the foreground color if <paramref name="toggle"/> is true,
    /// or <c>ESC[48;2;R;G;Bm</c> to set the background color if <paramref name="toggle"/> is false.
    /// </summary>
    public void Color(RGB color, bool toggle)
    {
        _buffer[_pointer++] = Escape;
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = toggle ? (byte)'3' : (byte)'4';
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

        if (toggle) Foreground = color; else Background = color;
    }


    /// <summary>
    /// Emits <c>ESC[2J</c> to clear the screen.
    /// </summary>
    public void Clear()
    {
        _buffer[_pointer++] = Escape;
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)'J';
    }

    /// <summary>
    /// Emits <c>ESC[0m</c> to reset text attributes, colors and styles.
    /// </summary>
    public void Reset()
    {
        _buffer[_pointer++] = Escape;
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'0';
        _buffer[_pointer++] = (byte)'m';
    }

    /// <summary>
    /// Emits <c>ESC[?1049h</c> to enter the alternate buffer if <paramref name="toggle"/> is true,
    /// or <c>ESC[?1049l</c> to leave the alternate buffer if <paramref name="toggle"/> is false.
    /// </summary>
    public void Alternate(bool toggle)
    {
        _buffer[_pointer++] = Escape;
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'?';
        _buffer[_pointer++] = (byte)'1';
        _buffer[_pointer++] = (byte)'0';
        _buffer[_pointer++] = (byte)'4';
        _buffer[_pointer++] = (byte)'9';
        _buffer[_pointer++] = toggle ? (byte)'h' : (byte)'l';
    }

    /// <summary>
    /// Emits <c>ESC[?25h</c> to show the cursor if <paramref name="toggle"/> is true,
    /// or <c>ESC[?25l</c> to hide the cursor if <paramref name="toggle"/> is false.
    /// </summary>
    public void Cursor(bool toggle)
    {
        _buffer[_pointer++] = Escape;
        _buffer[_pointer++] = (byte)'[';
        _buffer[_pointer++] = (byte)'?';
        _buffer[_pointer++] = (byte)'2';
        _buffer[_pointer++] = (byte)'5';
        _buffer[_pointer++] = toggle ? (byte)'h' : (byte)'l';
    }

    public void Write(byte value)
    {
        _buffer[_pointer++] = value;
        Position += new Int2(1, 0);
    }

    public void Flush()
    {
        if (_pointer == 0) return;
        _output.Write(_buffer, 0, _pointer);
        _pointer = 0;
    }
}
