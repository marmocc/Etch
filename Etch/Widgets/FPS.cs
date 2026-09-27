using Etch.Common;
using Etch.Terminal;

namespace Etch.Widgets;

public class FPS(int intervalSeconds) : IWidget
{
    public const int Width = 6; // "XXXXXX"
    public static ReadOnlySpan<byte> Default => "??????"u8;
    public readonly int IntervalSeconds = intervalSeconds;
    private float _accumulatedSeconds;

    public Int2 Size { get; } = new(Width, 1);

    public void Render(Context context)
    {
        var frametimeBuffer = context.Logger.FrametimeBuffer;
        if (frametimeBuffer.Count == 0) return;

        _accumulatedSeconds += frametimeBuffer.Last;
        if (_accumulatedSeconds < IntervalSeconds) return;
        _accumulatedSeconds -= IntervalSeconds;

        float sum = 0f;
        foreach (float f in frametimeBuffer.Head) sum += f;
        foreach (float f in frametimeBuffer.Tail) sum += f;
        float averageFrametime = sum / frametimeBuffer.Count;

        int fps = averageFrametime > 0f ? (int)(1f / averageFrametime) : 0;
        Span<byte> buffer = context.Prepare(Int2.Zero, Width, Color.White);
        if (buffer.IsEmpty) return;
        int written = buffer.Length;
        if (fps >= 1_000_000 || fps <= 0) Default.CopyTo(buffer);
        else written = Format.Into(buffer, fps);
        context.Commit(written);
    }
}
