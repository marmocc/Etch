using Etch.Graphics;
using System.Diagnostics;

namespace Etch;

public class Etcher(int width, int height)
{
    private readonly Writer _writer = new(8192);
    private Frame _front = new(width, height);
    private Frame _back = new(width, height);
    private readonly Frame.Delta[] _diff = new Frame.Delta[width * height];

    public Frame Frame => _front;

    private readonly Stopwatch _stopwatch = new();
    public long LastDiffTicks { get; private set; }
    public long LastWriteTicks { get; private set; }
    public long LastOverlayTicks { get; private set; }
    public long LastFlushTicks { get; private set; }
    public int Frames { get; private set; }

    public void Render(Stream stream, bool overlay = false)
    {
        _stopwatch.Restart();

        int count = Frame.Diff(_front, _back, _diff);

        LastDiffTicks = _stopwatch.ElapsedTicks;

        foreach (var delta in _diff[..count])
        {
            _writer.Move(delta.X, delta.Y);
            _writer.Color(delta.Color);
            _writer.Write(delta.Color.Glyph);
        }

        LastWriteTicks = _stopwatch.ElapsedTicks;

        if (overlay) Overlay();

        LastOverlayTicks = _stopwatch.ElapsedTicks;

        _writer.Flush(stream);
        
        LastFlushTicks = _stopwatch.ElapsedTicks;

        (_front, _back) = (_back, _front);
        _front.Clear();

        _stopwatch.Stop();
    }

    private void Overlay()
    {
        if (Frames++ % 60 != 0) return;

        _writer.Move(0, _front.Height);
        _writer.Color(Color.White);

        _writer.Write("["u8);
        _writer.Write(_front.Width);
        _writer.Write("x"u8);
        _writer.Write(_front.Height);
        _writer.Write("] diff: "u8);
        _writer.Write(LastDiffTicks);
        _writer.Write(" - write: "u8);
        _writer.Write(LastWriteTicks);
        _writer.Write(" - overlay: "u8);
        _writer.Write(LastOverlayTicks);
        _writer.Write(" - flush: "u8);
        _writer.Write(LastFlushTicks);
    }
}
