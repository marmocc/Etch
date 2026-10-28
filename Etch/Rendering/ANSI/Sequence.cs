using Etch.Common;
using System.Buffers.Text;
using System.Runtime.CompilerServices;

namespace Etch.Rendering.ANSI;

public static class Sequence
{
    public const byte Escape = 0x1B;

    /// <summary>
    /// Emits <c>ESC[Y;XH</c> to set the cursor position.
    /// </summary>
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

    /// <summary>
    /// Emits <c>ESC[38;2;R;G;Bm</c> to set the foreground color to <paramref name="color"/>.
    /// </summary>
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

    /// <summary>
    /// Emits <c>ESC[48;2;R;G;Bm</c> to set the background color to <paramref name="color"/>.
    /// </summary>
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

    /// <summary>
    /// Emits <c>ESC[2J</c> to clear the screen.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Clear(Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)'J';
    }

    /// <summary>
    /// Emits <c>ESC[0m</c> to reset text attributes, colors and styles.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Reset(Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'0';
        buffer[written++] = (byte)'m';
    }

    /// <summary>
    /// Emits <c>ESC[?1049h</c> to enter the alternate buffer if <paramref name="toggle"/> is true,
    /// or <c>ESC[?1049l</c> to leave the alternate buffer if <paramref name="toggle"/> is false.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Alternate(bool toggle, Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'?';
        buffer[written++] = (byte)'1';
        buffer[written++] = (byte)'0';
        buffer[written++] = (byte)'4';
        buffer[written++] = (byte)'9';
        buffer[written++] = toggle ? (byte)'h' : (byte)'l';
    }

    /// <summary>
    /// Emits <c>ESC[?25h</c> to show the cursor if <paramref name="toggle"/> is true,
    /// or <c>ESC[?25l</c> to hide the cursor if <paramref name="toggle"/> is false.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Cursor(bool toggle, Span<byte> buffer, ref int written)
    {
        buffer[written++] = Escape;
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'?';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)'5';
        buffer[written++] = toggle ? (byte)'h' : (byte)'l';
    }

    /// <summary>
    /// Emits <paramref name="value"/> in the buffer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(byte value, Span<byte> buffer, ref int written)
    {
        buffer[written++] = value;
    }
}