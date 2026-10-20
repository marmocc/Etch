using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Etch.Backend;
using Etch.Backend.ANSI;
using Etch.Common;
using Etch.Painters;

namespace Etch.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<RenderLoopBenchmarks>();
    }
}

[MemoryDiagnoser]
public class RenderLoopBenchmarks
{
    // Using 'default!' to suppress the unassigned variable warnings.
    // These are guaranteed to be initialized in [GlobalSetup].
    private Surface _surface = default!;
    private Chaos _painter = default!;
    private Etchel[] _etchels = default!;
    private Int2 _size = new(80, 40);

    [GlobalSetup]
    public void Setup()
    {
        using var ms = new MemoryStream();
        _surface = new Surface(_size, ms);
        _painter = new Chaos();
        _etchels = new Etchel[_size.X * _size.Y];
    }

    [Benchmark]
    public void Tick()
    {
        // By calling _painter.Paint(context) where _painter is typed as 'Chaos',
        // the JIT can devirtualize this call, matching the performance of the generic Renderer.
        Context context = new(0, TimeSpan.Zero, TimeSpan.Zero, _size, _etchels);
        _painter.Paint(context);
        _surface.Present(_etchels);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
    }
}
