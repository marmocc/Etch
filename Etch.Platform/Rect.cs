namespace Etch.Platform;

public readonly record struct Rect(Float2 Position, Float2 Size)
{
    public float X => Position.X;
    public float Y => Position.Y;
    public float Width => Size.X;
    public float Height => Size.Y;
}