using Etch.Common;

namespace Etch.Graphics;

public sealed class Canvas(Int2 size)
{
    public Int2 Size { get; } = size;
    public Color[] Data { get; } = new Color[size.X * size.Y];
    // Will be expanded to support layers.

    public void Clear() => Array.Clear(Data);
}