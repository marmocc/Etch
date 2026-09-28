using Etch.Common;

namespace Etch.Graphics;

public readonly ref struct Brush(Span<Color> destination, Int2 available, float deltaTime)
{
    private readonly Span<Color> _destination = destination;
    public readonly Int2 Available = available;
    public readonly float DeltaTime = deltaTime;

    public void Draw(Flat index, Color color) => _destination[index.Value] = color;
    public void Draw(Int2 position, Color color) => Draw(Flat.Flatten(position, Available), color);
}
