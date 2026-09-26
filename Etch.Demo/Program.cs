using Etch;
using Etch.Common;
using Etch.Graphics;
using System.Diagnostics;

int width = args.Length > 0 ? int.Parse(args[0]) : 80;
int height = args.Length > 1 ? int.Parse(args[1]) : 40;
Int2 size = new(width, height);


var etcher = new Etcher(size);
var canvas = new Canvas(size);
etcher.Add(canvas);

var stream = Console.OpenStandardOutput();
var frameStopwatch = Stopwatch.StartNew();

float time = 0;
Console.CursorVisible = false;
Console.Clear();

while (true)
{
    float deltaTime = (float)frameStopwatch.Elapsed.TotalSeconds;
    frameStopwatch.Restart();
    time += deltaTime;

    float t1 = time * 1.2f;
    float t2 = time * 0.8f;

    for (int y = 0; y < height; y++)
    {
        float ny = (float)y / height;

        for (int x = 0; x < width; x++)
        {
            float nx = (float)x / width;

            float u = MathF.Sin(nx * 4.0f + t1) + MathF.Cos(ny * 4.0f - t2);
            float v = MathF.Cos(nx * 3.0f - t2) + MathF.Sin(ny * 5.0f + t1);

            float cx = nx * 6.0f + u * 2.0f;
            float cy = ny * 6.0f + v * 2.0f;

            float pseudoDist = MathF.Abs(cx) + MathF.Abs(cy) - (MathF.Min(MathF.Abs(cx), MathF.Abs(cy)) * 0.5f);

            float n = MathF.Sin(cx + t1)
                    + MathF.Cos(cy + t2)
                    + MathF.Sin(pseudoDist - t1 * 1.5f);

            float intensity = (n + 3.0f) / 6.0f;
            intensity = Math.Clamp(intensity, 0.0f, 1.0f);

            byte r = (byte)(MathF.Sin(intensity * MathF.PI + time) * 127 + 128);
            byte g = (byte)(intensity * 255);
            byte b = (byte)(MathF.Cos(intensity * MathF.PI * 0.5f) * 200 + 55);

            Color color = new(r, g, b, 255);
            Int2 position = new(x, y);
            canvas.Draw(position, color);
        }
    }

    etcher.Render(stream);
}