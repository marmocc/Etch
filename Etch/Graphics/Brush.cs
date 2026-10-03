using Etch.Common;

namespace Etch.Graphics;

public readonly ref struct Brush(Span<Pixel> destination, Int2 available, float deltaTime)
{
    private readonly Span<Pixel> _destination = destination;
    public readonly Int2 Available = available;
    public readonly float DeltaTime = deltaTime;

    public void Draw(Flat index, Pixel pixel) => _destination[index.Value] = pixel;
    public void Draw(Int2 position, Pixel pixel) => Draw(Flat.Flatten(position, Available), pixel);
}