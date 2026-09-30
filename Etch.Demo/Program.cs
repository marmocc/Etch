using Etch.Common;
using Etch.Graphics;
using Etch.Graphics.Painters;
using Etch.Platform;
using Etch.Terminal;

int height = Console.WindowHeight;
Int2 size = new(height * 2, height);

var canvas = new Canvas(size, Writer.Terminal, new Chaos());
if (OperatingSystem.IsWindows()) ANSI.Enable();
Console.CursorVisible = false;
Console.Clear();

while (true)
    canvas.Render();