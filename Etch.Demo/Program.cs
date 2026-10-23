using Etch.Rendering.ANSI;
using Etch.Painters;
using Etch.Rendering;
using Etch.Diagnostics;

Chaos painter = new();
using Surface surface = new();
Context context = new(surface.Size);

Benchmark<Surface, Chaos> benchmark = new(surface, painter, context);
var (avgPaint, avgPresent, avgFlush) = benchmark.Run(10_000, 10_000);
surface.Dispose();

Console.WriteLine($"[{surface.Size.X} x {surface.Size.Y}] " +
    $"{avgPaint.TotalMicroseconds}us - {avgPresent.TotalMicroseconds}us - {avgFlush.TotalMicroseconds}us");