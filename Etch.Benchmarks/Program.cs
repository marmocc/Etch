using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace Etch.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BytesColumn.ClearAll();
        var switcher = BenchmarkSwitcher.FromTypes(
        [
            typeof(ChaosBenchmarks),
            typeof(SpiralBenchmarks)
        ]);
        switcher.Run(args, DefaultConfig.Instance.AddColumn(new BytesColumn()));
    }
}