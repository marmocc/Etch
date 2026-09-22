using Etch.Geometry;
using System.Buffers;

namespace Etch.Graphics.Terminal;

public readonly ref struct Context(ArrayBufferWriter<byte> writer) : IContext
{
    public void Clear(Color fill)
    {
        throw new NotImplementedException();
    }

    public void Plot(Vector2<int> position, Color color)
    {
        throw new NotImplementedException();
    }

    public void Rectangle(Rectangle2D<int> rectangle, Color color)
    {
        throw new NotImplementedException();
    }

    public void Segment(Segment2D<int> segment, Color color)
    {
        throw new NotImplementedException();
    }

    public void Triangle(Triangle2D<int> triangle, Color color)
    {
        throw new NotImplementedException();
    }

    public void Write(Vector2<int> position, ReadOnlySpan<char> text, Color color)
    {
        throw new NotImplementedException();
    }
}