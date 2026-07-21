namespace Etch.UI;

using Etch.Platform.SkiaOnSilk;

// Untyped Etcher for convenience, defaults to SkiaOnSilk
public sealed class Etcher
{
    public static Etcher<SkiaContext, SilkWindow> Create(string title, int width, int height)
        => Etcher<SkiaContext, SilkWindow>.Create(new SilkWindow(title, width, height));
}
