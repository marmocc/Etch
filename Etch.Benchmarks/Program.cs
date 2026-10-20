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
    private Surface _surface = default!;
    private MemoryStream _stream = default!;
    private Etchel[] _etchels = default!;
    private Int2 _size = new(80, 40);
    private ulong _frame = 0;

    [GlobalSetup]
    public void Setup()
    {
        _stream = new MemoryStream();
        _surface = new Surface(_size, _stream);
        _etchels = new Etchel[_size.X * _size.Y];
    }

    [Benchmark]
    public void Tick()
    {
        Chaos painter = new();
        Context context = new(_frame, TimeSpan.Zero, TimeSpan.Zero, _size, _etchels);
        painter.Paint(context);
        _surface.Present(_etchels);
        _frame++;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
        _stream.Dispose();
    }
}
