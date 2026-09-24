using BenchmarkDotNet.Attributes;
using Etch.Graphics;

namespace Etch.Benchmarks;

[MemoryDiagnoser]
public class ColorBenchmarks
{
    private Color _color1;
    private Color _color2;

    [GlobalSetup]
    public void Setup()
    {
        _color1 = Color.Green.WithAlpha(150);
        _color2 = Color.Red.WithAlpha(150);
    }

    [Benchmark]
    public bool Equals() => _color1.Equals(_color2);

    [Benchmark]
    public Color Blend() => Color.Blend(_color1, _color2);
    
    [Benchmark]
    public byte Density() => _color1.Density;
}