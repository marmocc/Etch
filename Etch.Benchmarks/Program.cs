using BenchmarkDotNet.Running;

namespace Etch.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        var switcher = BenchmarkSwitcher.FromTypes(
        [
            typeof(ChaosBenchmarks),
            typeof(SpiralBenchmarks)
        ]);
        switcher.Run(args);
    }
}