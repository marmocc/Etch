using Etch.Common;
using Etch.Graphics;
using Etch.Backend.Ansi;

Int2 size = new(Console.WindowWidth, Console.WindowHeight);

Console.Clear();
Console.CursorVisible = false;
if (OperatingSystem.IsWindows()) Ansi.Enable();
var canvas = new Canvas(size);

int frame = 0;
while (true)
{
    frame++;
    for(int y = 0; y < size.Y; y++)
        for (int x = 0; x < size.X; x++)
            canvas.Data[Flat.Flatten(x, y, size.X).Value] = Painters.Chaos(x, y, frame);
    Surface.Default.Present(canvas);
}