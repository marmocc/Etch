using System.Buffers;
using System.Buffers.Text;

namespace Etch.Terminal;

public static class Format
{
    public static int Into(Span<byte> buffer, int value) { Utf8Formatter.TryFormat(value, buffer, out int written); return written; }
    public static int Into(Span<byte> buffer, float value, StandardFormat format = default) { Utf8Formatter.TryFormat(value, buffer, out int written, format); return written; }
}