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
    Console.WriteLine($"{Console.WindowWidth} x {Console.WindowHeight}");
    Console.WriteLine($"{paint.Microseconds}us - {present.Microseconds}us - {flush.Microseconds}us");
}
else
{
    Console.WriteLine("Etch.Demo");
    Console.WriteLine("'--demo' to run a demonstration.");
    Console.WriteLine("'--benchmark' to run a benchmark.");
}