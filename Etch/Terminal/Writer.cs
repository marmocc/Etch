using Etch.Common;
using System.Buffers;
using System.Buffers.Text;

namespace Etch.Terminal;

public sealed class Writer
{
    private readonly ArrayBufferWriter<byte> _output;

    private Int2 _cursor;
    private Color _foreground;
    private Color _background;

    public Writer(int initialCapacity) : this(initialCapacity, Int2.One, Color.White, Color.Black) { }
    public Writer(int initialCapacity, Int2 initialCursor, Color initialForeground, Color initialBackground)
    {
        _output = new(initialCapacity);
        InternalMove(initialCursor);
        _cursor = initialCursor;
        InternalForeground(initialForeground);
        _foreground = initialForeground;
        InternalBackground(initialBackground);
        _background = initialBackground;
    }


    private void InternalMove(Int2 oneIndexedPosition)
    {
        Span<byte> buffer = _output.GetSpan(16);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(oneIndexedPosition.Y, buffer[written..], out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        written += rowWritten;

        buffer[written++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(oneIndexedPosition.X, buffer[written..], out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        written += colWritten;

        buffer[written++] = (byte)'H';
        _output.Advance(written);
    }
    private void InternalForeground(Color foreground)
    {
        Span<byte> buffer = _output.GetSpan(24);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'3'; // 3 for Foreground
        buffer[written++] = (byte)'8';
        buffer[written++] = (byte)';';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)';';

        bool rSuccess = Utf8Formatter.TryFormat(foreground.R, buffer[written..], out int rWritten);
        if (!rSuccess) throw new InvalidOperationException("Failed to format red value.");
        written += rWritten;

        buffer[written++] = (byte)';';

        bool gSuccess = Utf8Formatter.TryFormat(foreground.G, buffer[written..], out int gWritten);
        if (!gSuccess) throw new InvalidOperationException("Failed to format green value.");
        written += gWritten;

        buffer[written++] = (byte)';';

        bool bSuccess = Utf8Formatter.TryFormat(foreground.B, buffer[written..], out int bWritten);
        if (!bSuccess) throw new InvalidOperationException("Failed to format blue value.");
        written += bWritten;

        buffer[written++] = (byte)'m';
        _output.Advance(written);
    }
    private void InternalBackground(Color background)
    {
        Span<byte> buffer = _output.GetSpan(24);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'4'; // 4 for Background
        buffer[written++] = (byte)'8';
        buffer[written++] = (byte)';';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)';';

        bool rSuccess = Utf8Formatter.TryFormat(background.R, buffer[written..], out int rWritten);
        if (!rSuccess) throw new InvalidOperationException("Failed to format red value.");
        written += rWritten;

        buffer[written++] = (byte)';';

        bool gSuccess = Utf8Formatter.TryFormat(background.G, buffer[written..], out int gWritten);
        if (!gSuccess) throw new InvalidOperationException("Failed to format green value.");
        written += gWritten;

        buffer[written++] = (byte)';';

        bool bSuccess = Utf8Formatter.TryFormat(background.B, buffer[written..], out int bWritten);
        if (!bSuccess) throw new InvalidOperationException("Failed to format blue value.");
        written += bWritten;

        buffer[written++] = (byte)'m';
        _output.Advance(written);
    }

    public void Move(Int2 zeroIndexedPosition)
    {
        Int2 oneIndexedPosition = zeroIndexedPosition + 1;
        if (_cursor == oneIndexedPosition) return;
        InternalMove(oneIndexedPosition);
        _cursor = oneIndexedPosition;
    }

    public void Foreground(Color foreground)
    {
        if (_foreground == foreground) return;
        InternalForeground(foreground);
        _foreground = foreground;
    }

    public void Background(Color background)
    {
        if (_background == background) return;
        InternalBackground(background);
        _background = background;
    }

    public void Write(byte value)
    {
        Span<byte> buffer = _output.GetSpan(1);
        buffer[0] = value;
        _output.Advance(1);

        _cursor = _cursor.AddX(1);
    }

    public void Write(ReadOnlySpan<byte> text)
    {
        Span<byte> buffer = _output.GetSpan(text.Length);
        text.CopyTo(buffer);
        _output.Advance(text.Length);

        _cursor = _cursor.AddX(text.Length);
    }

    public void Write(int value)
    {
        Span<byte> buffer = _output.GetSpan(16);
        bool intSuccess = Utf8Formatter.TryFormat(value, buffer, out int written);
        if (!intSuccess) throw new InvalidOperationException("Failed to format int value.");
        _output.Advance(written);

        _cursor = _cursor.AddX(written);
    }

    public void Write(long value)
    {
        Span<byte> buffer = _output.GetSpan(16);
        bool longSuccess = Utf8Formatter.TryFormat(value, buffer, out int written);
        if (!longSuccess) throw new InvalidOperationException("Failed to format int value.");
        _output.Advance(written);

        _cursor = _cursor.AddX(written);
    }

    public void Write(float value, StandardFormat format = default)
    {
        Span<byte> buffer = _output.GetSpan(16);
        bool floatSuccess = Utf8Formatter.TryFormat(value, buffer, out int written, format);
        if (!floatSuccess) throw new InvalidOperationException("Failed to format float value.");
        _output.Advance(written);

        _cursor = _cursor.AddX(written);
    }

    public void Write(double value, StandardFormat format = default)
    {
        Span<byte> buffer = _output.GetSpan(16);
        bool doubleSuccess = Utf8Formatter.TryFormat(value, buffer, out int written, format);
        if (!doubleSuccess) throw new InvalidOperationException("Failed to format float value.");
        _output.Advance(written);

        _cursor = _cursor.AddX(written);
    }

    public void Flush(Stream output)
    {
        output.Write(_output.WrittenSpan);
        _output.Clear();
    }
}