using System.Diagnostics;
using Etch.Geometry;
using Etch.Graphics;
using Etch.Graphics.Grid;

const int width = 80;
const int height = 40;

var surface = new Surface(new Vector2<int>(width, height));
var context = surface.Context;

var stopwatch = Stopwatch.StartNew();
double time = 0;
int frameCount = 0;
double fpsTimer = 0;
double fps = 0;

Vector2<int> center = new(width / 2, height / 2);
const int orbiterCount = 100;

var rng = new Random(1);
var starPositions = new (int x, int y, byte brightness)[150];
for (int i = 0; i < starPositions.Length; i++)
    starPositions[i] = (rng.Next(width), rng.Next(height), (byte)rng.Next(60, 200));

while (true)
{
    double deltaTime = stopwatch.Elapsed.TotalSeconds;
    stopwatch.Restart();
    time += deltaTime;

    fpsTimer += deltaTime;
    frameCount++;
    if (fpsTimer >= 0.5) { fps = frameCount / fpsTimer; frameCount = 0; fpsTimer = 0; }

    context.Clear(new Color(8, 8, 16));

    // Static starfield background
    foreach (var (x, y, b) in starPositions)
        context.Plot(new Vector2<int>(x, y), new Color(b, b, b));

    // Multiple orbiters at different radii/speeds, connected by translucent lines to center
    for (int i = 0; i < orbiterCount; i++)
    {
        double speed = 1.0 + i * 0.4;
        double radius = 6 + i * 4;
        double a = time * speed + i;

        var pos = new Vector2<int>(
            center.X + (int)(Math.Cos(a) * radius),
            center.Y + (int)(Math.Sin(a) * radius * 0.5));

        byte hue = (byte)(100 + i * 25);
        context.Segment(new Segment2D<int>(center, pos), new Color(hue, hue, 255, 90));
        context.Plot(pos, new Color(255, (byte)(255 - i * 30), 0));
    }

    context.Write(new Vector2<int>(1, 1), $"FPS: {fps:F1}  t={time:F1}s", Color.White);

    surface.Present();
}