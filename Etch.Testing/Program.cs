using Etch.Platform.SkiaOnSilk;
using Etch.Platform;

var window = new SilkWindow("Test Window", 800, 600);
window.Rendering += context =>
{
    context.Clear(new Color(255, 255, 255, 255));
    context.FillRect(new Rect(new(100, 100), new(200, 150)), new Color(255, 0, 0, 255));
};
window.Run();