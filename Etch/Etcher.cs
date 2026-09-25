using Etch.Graphics;

namespace Etch;

public class Etcher(int width, int height)
{
    private readonly Writer _writer = new(8192);
    private Frame _front = new(width, height);
    private Frame _back = new(width, height);
    private readonly Frame.Delta[] _diff = new Frame.Delta[width * height];

    public Frame Frame => _front;

    public void Render(Stream stream)
    {
        _writer.Move(0, 0);
        int count = Frame.Diff(_front, _back, _diff);
        foreach (var delta in _diff[..count])
        {
            _writer.Move(delta.X, delta.Y);
            _writer.Color(delta.Color);
            _writer.Write(delta.Color.Density);
        }

        _writer.Flush(stream);

        (_front, _back) = (_back, _front);
        _front.Clear();
    }
}
