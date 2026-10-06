using BenchmarkDotNet.Attributes;
using Etch.Backend.ANSI;
using Etch.Common;
using Etch.Graphics;

namespace Etch.Benchmarks;

[MemoryDiagnoser]
public class SpiralBenchmarks
{
    [Params(80)] public int Width;
    [Params(40)] public int Height;

    private const float FakeDeltaTime = 1f / 144f;
    private float _time;

    private Surface? _surface;
    private Canvas? _canvas;
    private MemoryStream? _etchStream;

    private MemoryStream? _rawStream;
    private Writer? _rawWriter;

    [GlobalSetup]
    public void Setup()
    {
        Int2 size = new(Width, Height);
        _time = 0;

        _etchStream = new MemoryStream(Width * Height * 64);
        _surface = new Surface(size, _etchStream);
        _canvas = new Canvas(size);

        _rawStream = new MemoryStream(Width * Height * 64);
        _rawWriter = new Writer(Width * Height * 64, _rawStream);
    }

    [GlobalCleanup(Target = nameof(Etch))]
    public void SaveEtchBytes() =>
        BytesColumn.Save(nameof(SpiralBenchmarks), nameof(Etch), _etchStream!.Length);

    [GlobalCleanup(Target = nameof(Raw))]
    public void SaveRawBytes() =>
        BytesColumn.Save(nameof(SpiralBenchmarks), nameof(Raw), _rawStream!.Length);


    [Benchmark]
    public void Etch()
    {
        _time += FakeDeltaTime;
        _etchStream!.SetLength(0);
        _etchStream.Position = 0;

        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                _canvas!.Data[Flat.Flatten(x, y, Width).Value] = Painters.Spiral(x, y, Width, Height, _time);
        _surface!.Present(_canvas!);
    }

    [Benchmark(Baseline = true)]
    public void Raw()
    {
        _time += FakeDeltaTime;
        _rawStream!.SetLength(0);
        _rawStream.Position = 0;

        for (int y = 0; y < Height; y++)
        {
            _rawWriter!.Move(new(0, y));
            for (int x = 0; x < Width; x++)
            {
                Color color = Painters.Spiral(x, y, Width, Height, _time);
                _rawWriter!.Foreground(color);
                _rawWriter!.Write(Surface.Density(color));
            }
        }

        _rawWriter!.Flush();
    }
}
