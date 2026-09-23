using Etch.Geometry;

namespace Etch.Graphics;

public interface IContext
{
    void Plot(Vector2<int> position, Color color);
    void Segment(Segment2D<int> segment, Color color);
    void Triangle(Triangle2D<int> triangle, Color color);
    void Rectangle(Rectangle2D<int> rectangle, Color color);
}