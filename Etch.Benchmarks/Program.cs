using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Etch.Common;

namespace Etch.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<EtchelBenchmarks>();
    }
}

[MemoryDiagnoser]
public class EtchelBenchmarks
{
    private Glyph _glyph = new((byte)65);
    private RGB _fg = new(255, 0, 0);
    private RGB _bg = new(0, 0, 255);

    [Benchmark]
    public Etchel CreateEtchel()
    {
        return new Etchel(_glyph, _fg, _bg);
    }
}
