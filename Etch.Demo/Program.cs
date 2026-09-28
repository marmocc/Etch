using Etch;
using Etch.Common;
using Etch.Graphics.Painters;
using Etch.Widgets;

int width = args.Length > 0 ? int.Parse(args[0]) : 80;
int height = args.Length > 1 ? int.Parse(args[1]) : 40;
Int2 size = new(width, height);

var etcher = new Etcher();
var canvas = new Canvas(size, new Spiral());

etcher.Add(new(0, 0), canvas);
etcher.Add(new(0, height), new FPS(1));

var stream = Console.OpenStandardOutput();
Console.CursorVisible = false;
Console.Clear();

while (true)
    etcher.Render(stream);