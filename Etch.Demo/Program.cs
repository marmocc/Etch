using Etch.Graphics;
using System.Diagnostics;

// Usage:
//   dotnet run -- <mode> <width> <height> [param]
//
// modes:
//   orbiters   [count]      - original demo scene (default param: 6)
//   chaos                   - every pixel changes every frame (worst case for diffing)
//   shapes     [count]      - N bouncing triangles + N bouncing segments (default param: 200)
//   sparse                  - static starfield + one moving pixel (best case for diffing)
//
// Examples:
//   dotnet run -- chaos 80 40
//   dotnet run -- shapes 120 50 1000
//   dotnet run -- sparse 200 60

string mode = args.Length > 0 ? args[0] : "chaos";
int width = args.Length > 1 ? int.Parse(args[1]) : 80;
int height = args.Length > 2 ? int.Parse(args[2]) : 40;
int param = args.Length > 3 ? int.Parse(args[3]) : -1;

var surface = new Surface(width, height);

var drawStopwatch = new Stopwatch();
var presentStopwatch = new Stopwatch();
var frameStopwatch = Stopwatch.StartNew();

double time = 0;
int frameCount = 0;
double fpsTimer = 0;
double fps = 0;
double avgDrawMs = 0;
double avgPresentMs = 0;

Console.CursorVisible = false;
Console.Clear();

while (true)
{
    double deltaTime = frameStopwatch.Elapsed.TotalSeconds;
    frameStopwatch.Restart();
    time += deltaTime;

    drawStopwatch.Restart();

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            double n = Math.Sin(x * 0.3 + time * 3) + Math.Cos(y * 0.3 - time * 2);
            byte v = (byte)((n + 2) / 4 * 255);
            Color color = new(v, (byte)(255 - v), (byte)(v / 2), 255);
            surface.Plot(x, y, color);
        }
    }

    drawStopwatch.Stop();

    presentStopwatch.Restart();
    surface.Present();
    presentStopwatch.Stop();

    fpsTimer += deltaTime;
    frameCount++;
    if (fpsTimer >= 0.5)
    {
        fps = frameCount / fpsTimer;
        avgDrawMs = drawStopwatch.Elapsed.TotalMilliseconds;
        avgPresentMs = presentStopwatch.Elapsed.TotalMilliseconds;
        frameCount = 0;
        fpsTimer = 0;

        Console.ResetColor();
        Console.SetCursorPosition(0, height);
        Console.Write($"mode={mode} size={width}x{height} param={param}  FPS: {fps:F1}  draw: {avgDrawMs:F3}ms  present: {avgPresentMs:F3}ms   ");
    }
}