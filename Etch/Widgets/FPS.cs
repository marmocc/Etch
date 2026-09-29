using Etch.Common;
using Etch.Terminal;

namespace Etch.Widgets;

public class FPS(int intervalSeconds) : IWidget
{
    public const int Width = 6; // "XXXXXX"
    public Int2 Size { get; } = new(Width, 1);
    public readonly int IntervalSeconds = intervalSeconds;

    private int _frames = 0;
    private float _time = 0f;
    public void Render(Context context)
    {
        _frames++;
        _time += context.DeltaTime;
        if (_time < IntervalSeconds) return;

        int fps = _time > 0f ? (int)(_frames / _time) : 0;
        _time = 0f;
        _frames = 0;

        Span<byte> buffer = context.Prepare(Int2.Zero, Width, Color.White, Color.Black);
        if (buffer.IsEmpty) return;

        buffer.Fill((byte)'.');
        if (fps >= 0 && fps <= 1_000_000) Format.Into(buffer, fps);
        context.Commit(buffer.Length);
    }
}