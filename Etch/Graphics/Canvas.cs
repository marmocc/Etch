using Etch.Common;

namespace Etch.Graphics;

public sealed class Canvas(Int2 size)
{
    public Int2 Size { get; } = size;
    public Color[] Data { get; private set; } = new Color[size.X * size.Y];
    // Will be expanded to support layers.

    public void Clear() => Array.Clear(Data);
    public void Swap(ref Color[] other) => (Data, other) = (other, Data);
}