namespace Etch.Graphics;

public interface IContext
{
    void Clear(Color fill);
    void Plot(Vector2<int> position, Color color);
    void Line(Vector2<int> positionA, Vector2<int> positionB, Color color);
    void Rectangle(Vector2<int> position, Vector2<int> size, Color color);
    void Write(Vector2<int> position, ReadOnlySpan<char> text, Color foreground, Color background);
}