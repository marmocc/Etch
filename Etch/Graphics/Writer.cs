using System.Buffers;
using System.Buffers.Text;
using System.Text;

namespace Etch.Graphics;

public class Writer(int initialCapacity)
{
    private Color _color = new();
    private bool _colorSet = false;
    private (int X, int Y) _cursor = new();
    private bool _cursorSet = false;
    private readonly ArrayBufferWriter<byte> _writer = new(initialCapacity);

    public void Move(int x, int y)
    {
        if (_cursorSet && _cursor == (x, y)) return;

        Span<byte> buffer = _writer.GetSpan(16);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(y + 1, buffer[written..], out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        written += rowWritten;

        buffer[written++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(x + 1, buffer[written..], out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        written += colWritten;

        buffer[written++] = (byte)'H';
        _writer.Advance(written);

        _cursorSet = true;
        _cursor = (x, y);
    }

    public void Color(Color color)
    {
        if (_colorSet && _color == color) return;

        Span<byte> buffer = _writer.GetSpan(24);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'3';
        buffer[written++] = (byte)'8';
        buffer[written++] = (byte)';';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)';';

        bool rSuccess = Utf8Formatter.TryFormat(color.R, buffer[written..], out int rWritten);
        if (!rSuccess) throw new InvalidOperationException("Failed to format red value.");
        written += rWritten;

        buffer[written++] = (byte)';';

        bool gSuccess = Utf8Formatter.TryFormat(color.G, buffer[written..], out int gWritten);
        if (!gSuccess) throw new InvalidOperationException("Failed to format green value.");
        written += gWritten;

        buffer[written++] = (byte)';';

        bool bSuccess = Utf8Formatter.TryFormat(color.B, buffer[written..], out int bWritten);
        if (!bSuccess) throw new InvalidOperationException("Failed to format blue value.");
        written += bWritten;

        buffer[written++] = (byte)'m';
        _writer.Advance(written);

        _colorSet = true;
        _color = color;
    }

    public void Write(byte value)
    {
        if (!_cursorSet) throw new InvalidOperationException("Cursor position is unknown. Unable to Write.");
        if (!_colorSet) throw new InvalidOperationException("Color is unknown. Unable to Write.");

        Span<byte> buffer = _writer.GetSpan(1);
        buffer[0] = value;
        _writer.Advance(1);

        _cursor = (_cursor.X + 1, _cursor.Y);
    }

    public void Write(ReadOnlySpan<byte> text)
    {
        if (!_cursorSet) throw new InvalidOperationException("Cursor position is unknown. Unable to Write.");
        if (!_colorSet) throw new InvalidOperationException("Color is unknown. Unable to Write.");

        _writer.Write(text);

        _cursor = (_cursor.X + text.Length, _cursor.Y);
    }

    public void Write(int value)
    {
        if (!_cursorSet) throw new InvalidOperationException("Cursor position is unknown. Unable to Write.");
        if (!_colorSet) throw new InvalidOperationException("Color is unknown. Unable to Write.");

        Span<byte> buffer = _writer.GetSpan(16);
        bool intSuccess = Utf8Formatter.TryFormat(value, buffer, out int written);
        if(!intSuccess) throw new InvalidOperationException("Failed to format int value.");
        _writer.Advance(written);

        _cursor = (_cursor.X + written, _cursor.Y);
    }

    public void Write(long value)
    {
        if (!_cursorSet) throw new InvalidOperationException("Cursor position is unknown. Unable to Write.");
        if (!_colorSet) throw new InvalidOperationException("Color is unknown. Unable to Write.");

        Span<byte> buffer = _writer.GetSpan(16);
        bool longSuccess = Utf8Formatter.TryFormat(value, buffer, out int written);
        if (!longSuccess) throw new InvalidOperationException("Failed to format int value.");
        _writer.Advance(written);

        _cursor = (_cursor.X + written, _cursor.Y);
    }

    public void Write(float value, StandardFormat format = default)
    {
        if (!_cursorSet) throw new InvalidOperationException("Cursor position is unknown. Unable to Write.");
        if (!_colorSet) throw new InvalidOperationException("Color is unknown. Unable to Write.");

        Span<byte> buffer = _writer.GetSpan(16);
        bool floatSuccess = Utf8Formatter.TryFormat(value, buffer, out int written, format);
        if(!floatSuccess) throw new InvalidOperationException("Failed to format float value.");
        _writer.Advance(written);

        _cursor = (_cursor.X + written, _cursor.Y);
    }

    public void Write(double value, StandardFormat format = default)
    {
        if (!_cursorSet) throw new InvalidOperationException("Cursor position is unknown. Unable to Write.");
        if (!_colorSet) throw new InvalidOperationException("Color is unknown. Unable to Write.");

        Span<byte> buffer = _writer.GetSpan(16);
        bool doubleSuccess = Utf8Formatter.TryFormat(value, buffer, out int written, format);
        if (!doubleSuccess) throw new InvalidOperationException("Failed to format float value.");
        _writer.Advance(written);

        _cursor = (_cursor.X + written, _cursor.Y);
    }

    public void Flush(Stream output)
    {
        output.Write(_writer.WrittenSpan);
        _writer.Clear();
    }
}
