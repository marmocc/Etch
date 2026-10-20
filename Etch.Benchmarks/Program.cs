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

public class MockPainter : IPainter
{
    public void Paint<TContext>(TContext context) where TContext : IContext, allows ref struct
    {
        context.Clear();
        context.Plot(new Int2(0, 0), Etchel.Solid(RGB.Red));
    }
}

[MemoryDiagnoser]
public class RenderLoopBenchmarks
{
    private Surface _surface;
    private MockPainter _painter = new();
    private TimeSpan _interval = TimeSpan.FromMilliseconds(16);

    [GlobalSetup]
    public void Setup()
    {
        // Use a MemoryStream to avoid Terminal/GPU overhead
        using var ms = new MemoryStream();
        _surface = new Surface(new Int2(1920, 1080), ms);
    }

    [Benchmark]
    public void FrameTick()
    {
        // We simulate the logic inside the while(true) loop of Renderer.Start
        // but only the parts that depend on the data layout and painting.

        int sizeX = _surface.Size.X;
        int sizeY = _surface.Size.Y;
        Etchel[] etchels = new Etchel[sizeX * sizeY];

        Context context = new(0, TimeSpan.Zero, TimeSpan.Zero, _surface.Size, etchels);
        _painter.Paint(context);
        _surface.Present(etchels);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
    }
}
