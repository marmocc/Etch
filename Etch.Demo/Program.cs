using Etch;
using Etch.Common;
using Etch.Graphics.Painters;
using Etch.Platform;
using Etch.Widgets;

int height = Console.WindowHeight - 1;
Int2 size = new(height * 2, height);

var etcher = new Etcher();
var canvas = new Canvas(size, new Spiral());

etcher.Add(new(0, 0), new FPS(1));
etcher.Add(new(0, 1), canvas);

ConsoleHost.EnableANSI();
var stream = Console.OpenStandardOutput();
Console.CursorVisible = false;
Console.Clear();

while (true)
    etcher.Render(stream);