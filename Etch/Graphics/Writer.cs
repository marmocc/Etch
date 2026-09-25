using System.Buffers;
using System.Buffers.Text;
using System.Text;

namespace Etch.Graphics;

public class Writer(int initialCapacity)
{
    private readonly ArrayBufferWriter<byte> _writer = new(initialCapacity);

    public void Move(int row, int col)
    {
        Span<byte> buffer = _writer.GetSpan(16);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(row, buffer[written..], out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        written += rowWritten;

        buffer[written++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(col, buffer[written..], out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        written += colWritten;

        buffer[written++] = (byte)'H';
        _writer.Advance(written);
    }

    public void Color(Color color, bool isForeground)
    {
        Span<byte> buffer = _writer.GetSpan(24);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = isForeground ? (byte)'3' : (byte)'4';
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
    }

    public void Clear()
    {
        Span<byte> buffer = _writer.GetSpan(8);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)'J';
        _writer.Advance(written);
    }

    public void Reset()
    {
        Span<byte> buffer = _writer.GetSpan(8);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'0';
        buffer[written++] = (byte)'m';
        _writer.Advance(written);
    }

    public void Write(byte value)
    {
        Span<byte> buffer = _writer.GetSpan(1);
        buffer[0] = value;
        _writer.Advance(1);
    }

    public void Write(ReadOnlySpan<char> text)
    {
        int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length);
        Span<byte> buffer = _writer.GetSpan(maxByteCount);
        int bytesWritten = Encoding.UTF8.GetBytes(text, buffer);
        _writer.Advance(bytesWritten);
    }

    public void NewLine()
    {
        Span<byte> buffer = _writer.GetSpan(1);
        buffer[0] = (byte)'\n';
        _writer.Advance(1);
    }

    public void Flush(Stream output)
    {
        output.Write(_writer.WrittenSpan);
        _writer.Clear();
    }
}
