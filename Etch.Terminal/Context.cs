using Etch.Common;

namespace Etch.Terminal;

public ref struct Context : IContext
{
    public void Clear(Color color)
    {
        throw new NotImplementedException();
    }

    public void Draw(Vector2<int> position, Vector2<int> size, Color color)
    {
        throw new NotImplementedException();
    }

    public void Plot(Vector2<int> position, Color color)
    {
        throw new NotImplementedException();
    }
}
