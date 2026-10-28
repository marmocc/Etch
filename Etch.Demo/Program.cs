using Etch;
using Etch.Painters;
using Etch.Rendering.ANSI;

if (args.Contains("--demo"))
{
    Etcher.Run<Surface, Spiral>(60);
}
else if (args.Contains("--benchmark"))
{
    var (paint, present, flush) = Etcher.Run<Surface>(10_000, 10_000);
    Console.WriteLine($"Etch.Demo - Benchmark");
    Console.WriteLine($"Size: {Console.WindowWidth}x{Console.WindowHeight}");
    Console.WriteLine($"Paint: {paint.TotalMicroseconds}us");
    Console.WriteLine($"Present: {present.TotalMicroseconds}us");
    Console.WriteLine($"Flush: {flush.TotalMicroseconds}us");
}
else
{
    Console.WriteLine("Etch.Demo - Help");
    Console.WriteLine("'--demo' to run a demonstration.");
    Console.WriteLine("'--benchmark' to run a benchmark.");
}