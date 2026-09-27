using Etch.Common;
using Etch.Terminal;

namespace Etch.Widgets;

public sealed class Canvas(Int2 size) : IWidget
{
    public Int2 Size { get; } = size;
    private Matrix<Color> _front = new(size);
    private Matrix<Color> _back = new(size);

    private readonly Matrix<Color>.Delta[] _diff = new Matrix<Color>.Delta[size.X * size.Y];

    public void Draw(Int2 position, Color color) =>
        _front[position] = Color.Blend(_front[position], color);

    public void Draw(Int2 position, int width, Color color) =>
        Color.Blend(_front[position, width], color);

    public void Render(Context context)
    {
        int count = Matrix<Color>.Diff(_front, _back, _diff);
        foreach (var delta in _diff[..count])
        {
            Span<byte> buffer = context.Prepare(delta.Position, 1, delta.Data);
            if (buffer.IsEmpty) continue;
            buffer[0] = delta.Data.Glyph;
            context.Commit(1);
        }

        (_front, _back) = (_back, _front);
        _front.Clear();
    }
}