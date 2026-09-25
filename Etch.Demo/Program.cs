using Etch;
using Etch.Graphics;
using System.Diagnostics;

int width = args.Length > 0 ? int.Parse(args[0]) : 80;
int height = args.Length > 1 ? int.Parse(args[1]) : 40;

var etcher = new Etcher(width, height);
var stream = Console.OpenStandardOutput();

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
            etcher.Draw(x, y, color);
        }
    }

    drawStopwatch.Stop();

    presentStopwatch.Restart();
    etcher.Render(stream);
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
        Console.Write($"[{width}x{height}] FPS: {fps:F1} - draw: {avgDrawMs:F3}ms - present: {avgPresentMs:F3}ms");
    }
}