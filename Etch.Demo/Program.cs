using Etch;
using Etch.Common;
using Etch.Widgets;
using System.Diagnostics;

int width = args.Length > 0 ? int.Parse(args[0]) : 80;
int height = args.Length > 1 ? int.Parse(args[1]) : 40;
Int2 size = new(width, height);

var etcher = new Etcher();
var canvas = new Canvas(size);
etcher.Add(new(0, 0), canvas);
etcher.Add(new(0, height), new FPS(1));

var stream = Console.OpenStandardOutput();
var drawStopwatch = Stopwatch.StartNew();

float time = 0;
Console.CursorVisible = false;
Console.Clear();

float centerX = width / 2.0f;
float centerY = height / 2.0f;
float aspect = width > 0 ? (float)height / width : 1.0f;

while (true)
{
    float drawTime = (float)drawStopwatch.Elapsed.TotalSeconds;
    drawStopwatch.Restart();
    time += drawTime;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            float dx = (x - centerX);
            float dy = (y - centerY) / aspect;

            float angle = MathF.Atan2(dy, dx);
            float radius = MathF.Sqrt(dx * dx + dy * dy);

            float n = MathF.Sin(radius * 0.5f - time * 3f + angle * 4f);
            byte v = (byte)((n + 1) / 2 * 255);

            Color color = new((byte)v, (byte)(v / 3), (byte)(255 - v), 255);
            canvas.Draw(new(x, y), color);
        }
    }

    drawStopwatch.Stop();
    etcher.Render(stream, drawTime);
}