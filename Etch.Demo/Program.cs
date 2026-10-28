using Etch;
using Etch.Painters;
using Etch.Rendering.ANSI;

if (args.Contains("--benchmark"))
{
    var (paint, present, flush) = Etcher.Run<Surface>(10_000, 10_000);
    Console.WriteLine($"[{Console.WindowWidth} x {Console.WindowHeight}] {paint.Microseconds}us - {present.Microseconds}us - {flush.Microseconds}us");
}
else
{
    Etcher.Run<Surface, Spiral>(60);
}