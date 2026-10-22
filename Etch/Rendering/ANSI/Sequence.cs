using Etch.Common;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace Etch.Rendering.ANSI;

public static class Sequence
{
    public const byte Escape = 0x1B;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Move(Int2 position, Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(position.Y + 1, buffer[written..], out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        written += rowWritten;

        buffer[written++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(position.X + 1, buffer[written..], out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        written += colWritten;

        buffer[written++] = (byte)'H';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Foreground(RGB color, Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'3'; // 3 for Foreground
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Background(RGB color, Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'4'; // 4 for Background
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
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)'J';
    }
}