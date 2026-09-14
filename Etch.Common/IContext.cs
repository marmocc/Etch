namespace Etch.Common;

public interface IContext
{
    void Clear(Color fill);
    void Plot(Vector2<int> position, Color color);
    void Draw(Vector2<int> position, Vector2<int> size, Color border, Color fill);
}