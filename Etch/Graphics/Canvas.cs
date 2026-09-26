using Etch.Common;
using Etch.Terminal;

namespace Etch.Graphics;

public sealed class Canvas(Int2 size) : IWidget
{
    public Int2 Size { get; } = size;
    private Frame _front = new(size);
    private Frame _back = new(size);

    private readonly Frame.Delta[] _diff = new Frame.Delta[size.X * size.Y];

    public void Draw(Int2 position, Color color) =>
        _front[position] = Color.Blend(_front[position], color);

    public void Draw(Int2 position, int width, Color color) =>
        Color.Blend(_front[position, width], color);

    public void Render(Context context)
    {
        int count = Frame.Diff(_front, _back, _diff);
        foreach (var delta in _diff[..count])
            context.Write(delta.Position, delta.Color, delta.Color.Glyph);

        (_front, _back) = (_back, _front);
        _front.Clear();
    }
}