using System.Buffers;
using System.Buffers.Text;
using System.Text;

namespace Etch.Graphics;

public static class ANSI
{
    public static void Move(ArrayBufferWriter<byte> writer, int row, int col)
    {
        Span<byte> buffer = writer.GetSpan(16);
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
        writer.Advance(written);
    }

    public static void Color(ArrayBufferWriter<byte> writer, Color color, bool isForeground)
    {
        Span<byte> buffer = writer.GetSpan(24);
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
        writer.Advance(written);        
    }

    public static void Clear(ArrayBufferWriter<byte> writer)
    {
        Span<byte> buffer = writer.GetSpan(8);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)'J';
        writer.Advance(written);
    }

    public static void Reset(ArrayBufferWriter<byte> writer)
    {
        Span<byte> buffer = writer.GetSpan(8);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'0';
        buffer[written++] = (byte)'m';
        writer.Advance(written);
    }

    public static void Write(ArrayBufferWriter<byte> writer, byte value)
    {
        Span<byte> buffer = writer.GetSpan(1);
        buffer[0] = value;
        writer.Advance(1);
    }

    public static void Write(ArrayBufferWriter<byte> writer, ReadOnlySpan<char> text)
    {
        int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length);
        Span<byte> buffer = writer.GetSpan(maxByteCount);
        int bytesWritten = Encoding.UTF8.GetBytes(text, buffer);
        writer.Advance(bytesWritten);
    }

    public static void NewLine(ArrayBufferWriter<byte> writer)
    {
        Span<byte> buffer = writer.GetSpan(1);
        buffer[0] = (byte)'\n';
        writer.Advance(1);
    }
}
