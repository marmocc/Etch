using SkiaSharp;

namespace Etch.Platform.SkiaOnSilk;

public readonly ref struct SkiaContext(SKCanvas canvas) : IContext
{
    private readonly SKCanvas _canvas = canvas;

    public void Clear(Color color) => _canvas.Clear(ToSk(color));
    public void FillRect(Rect rect, Color color)
    {
        using var paint = new SKPaint { Color = ToSk(color), IsAntialias = true };
        _canvas.DrawRect(ToSk(rect), paint);
    }

    private static SKColor ToSk(Color c) => new(c.R, c.G, c.B, c.A);
    private static SKRect ToSk(Rect r) => new(r.X, r.Y, r.X + r.Width, r.Y + r.Height);
}