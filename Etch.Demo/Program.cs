using Etch.Rendering.ANSI;
using Etch;

var (paint, present, flush) = Etcher.Run<Surface>(10_000, 10_000);
Console.WriteLine($"{paint.Microseconds}us - {present.Microseconds}us - {flush.Microseconds}us");