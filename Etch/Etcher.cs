using Etch.Geometry;
using Etch.Graphics;

namespace Etch;

public class Etcher(int width, int height)
{
    private readonly Writer _writer = new(8192);
    private Frame _front = new(width, height);
    private Frame _back = new(width, height);
    private readonly Frame.Delta[] _diff = new Frame.Delta[width * height];

    public readonly Plane Plane = new();

    public void Draw(int x, int y, Color color)
    {
        int index = y * _front.Width + x;
        Color.Blend(ref _front.Data[index], color);
    }

    public void Draw(int x, int y, int width, Color color)
    {
        int index = y * _front.Width + x;
        Color.Blend(_front.Data.AsSpan(index, width), color);
    }

    public void Render(Stream stream)
    {
        //Plane.RasterizeInto(_front);

        _writer.Move(0, 0);
        int count = Frame.Diff(_front, _back, _diff);
        foreach (var delta in _diff[..count])
        {
            _writer.Move(delta.Y, delta.X);
            _writer.Color(delta.Color, true);
            _writer.Write(delta.Color.Density);
        }

        _writer.Flush(stream);

        (_front, _back) = (_back, _front);
        _front.Clear();
    }
}
