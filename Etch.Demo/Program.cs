using Etch.Geometry;
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

var surface = new Surface(new Vector2<int>(width, height));
Vector2<int> center = new(width / 2, height / 2);

var drawStopwatch = new Stopwatch();
var presentStopwatch = new Stopwatch();
var frameStopwatch = Stopwatch.StartNew();

double time = 0;
int frameCount = 0;
double fpsTimer = 0;
double fps = 0;
double avgDrawMs = 0;
double avgPresentMs = 0;

var rng = new Random(1);

// --- Shared setup: starfield (used by orbiters + sparse) ---
var starPositions = new (int x, int y, byte brightness)[150];
for (int i = 0; i < starPositions.Length; i++)
    starPositions[i] = (rng.Next(width), rng.Next(height), (byte)rng.Next(60, 200));

// --- orbiters mode ---
int orbiterCount = mode == "orbiters" ? (param > 0 ? param : 6) : 0;

// --- shapes mode ---
int shapeCount = mode == "shapes" ? (param > 0 ? param : 200) : 0;
var triangles = new (double x, double y, double vx, double vy, double size, byte hue)[shapeCount];
var segments = new (double x, double y, double vx, double vy, double len, double angle, double spin, byte hue)[shapeCount];
for (int i = 0; i < shapeCount; i++)
{
    triangles[i] = (
        rng.NextDouble() * width, rng.NextDouble() * height,
        (rng.NextDouble() - 0.5) * 20, (rng.NextDouble() - 0.5) * 20,
        2 + rng.NextDouble() * 4,
        (byte)rng.Next(0, 255));

    segments[i] = (
        rng.NextDouble() * width, rng.NextDouble() * height,
        (rng.NextDouble() - 0.5) * 20, (rng.NextDouble() - 0.5) * 20,
        2 + rng.NextDouble() * 6,
        rng.NextDouble() * Math.PI * 2,
        (rng.NextDouble() - 0.5) * 4,
        (byte)rng.Next(0, 255));
}

// --- sparse mode ---
double sparseAngle = 0;

Console.CursorVisible = false;
Console.Clear();

while (true)
{
    var context = surface.Context;

    double deltaTime = frameStopwatch.Elapsed.TotalSeconds;
    frameStopwatch.Restart();
    time += deltaTime;

    drawStopwatch.Restart();

    switch (mode)
    {
        case "orbiters":
            DrawStarfield(context);
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
            break;

        case "chaos":
            // Every cell differs from last frame: forces Diff to return Length every time.
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double n = Math.Sin(x * 0.3 + time * 3) + Math.Cos(y * 0.3 - time * 2);
                    byte v = (byte)((n + 2) / 4 * 255);
                    context.Plot(new Vector2<int>(x, y), new Color(v, (byte)(255 - v), (byte)(v / 2), 255));
                }
            }
            break;

        case "shapes":
            for (int i = 0; i < shapeCount; i++)
            {
                var t = triangles[i];
                t.x += t.vx * deltaTime;
                t.y += t.vy * deltaTime;
                if (t.x < 0 || t.x > width) t.vx = -t.vx;
                if (t.y < 0 || t.y > height) t.vy = -t.vy;
                triangles[i] = t;

                var a = new Vector2<int>((int)t.x, (int)t.y);
                var b = new Vector2<int>((int)(t.x + t.size), (int)(t.y + t.size));
                var c = new Vector2<int>((int)(t.x - t.size), (int)(t.y + t.size));
                context.Triangle(new Triangle2D<int>(a, b, c), new Color(t.hue, (byte)(255 - t.hue), 128, 255));
            }
            for (int i = 0; i < shapeCount; i++)
            {
                var s = segments[i];
                s.x += s.vx * deltaTime;
                s.y += s.vy * deltaTime;
                s.angle += s.spin * deltaTime;
                if (s.x < 0 || s.x > width) s.vx = -s.vx;
                if (s.y < 0 || s.y > height) s.vy = -s.vy;
                segments[i] = s;

                var start = new Vector2<int>((int)s.x, (int)s.y);
                var end = new Vector2<int>(
                    (int)(s.x + Math.Cos(s.angle) * s.len),
                    (int)(s.y + Math.Sin(s.angle) * s.len));
                context.Segment(new Segment2D<int>(start, end), new Color(128, s.hue, (byte)(255 - s.hue), 255));
            }
            break;

        case "sparse":
            DrawStarfield(context);
            sparseAngle += deltaTime;
            var dotPos = new Vector2<int>(
                center.X + (int)(Math.Cos(sparseAngle) * (width / 3)),
                center.Y + (int)(Math.Sin(sparseAngle) * (height / 3)));
            context.Plot(dotPos, Color.Red);
            break;

        default:
            Console.WriteLine($"Unknown mode '{mode}'. Use: orbiters | chaos | shapes | sparse");
            return;
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

void DrawStarfield(Context ctx)
{
    foreach (var (x, y, b) in starPositions)
        ctx.Plot(new Vector2<int>(x, y), new Color(b, b, b));
}