using Etch.Backend;
using Etch.Common;
using Etch.Graphics;
using Etch.Graphics.Painters;
using Etch.Platform;

int height = Console.WindowHeight;
Int2 size = new(height * 2, height);

Console.Clear();
Console.CursorVisible = false;
if (OperatingSystem.IsWindows()) ANSI.Enable();
var canvas = new Canvas(size, Writer.Terminal, new Chaos());
while (true) canvas.Render();