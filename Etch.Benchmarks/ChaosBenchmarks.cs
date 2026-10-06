using BenchmarkDotNet.Attributes;
using Etch.Backend.ANSI;
using Etch.Common;
using Etch.Painters;

namespace Etch.Benchmarks;

[MemoryDiagnoser]
public class ChaosBenchmarks
{
    private const int FrameCount = 100;

    [Params(80)] public int Width;
    [Params(40)] public int Height;

    private Int2 _size;

    private Surface? _surface;
    private MemoryStream? _etchStream;

    private MemoryStream? _rawStream;
    private Writer? _rawWriter;

    [GlobalSetup]
    public void Setup()
    {
        _size = new(Width, Height);

        _etchStream = new MemoryStream(Width * Height * 32 * FrameCount);
        _surface = new Surface(_size, _etchStream);

        _rawStream = new MemoryStream(Width * Height * 32 * FrameCount);
        _rawWriter = new Writer(Width * Height * 64, _rawStream);
    }

    [GlobalCleanup(Target = nameof(Etch))]
    public void SaveEtchBytes() =>
        BytesColumn.Save(nameof(ChaosBenchmarks), nameof(Etch), _etchStream!.Length / FrameCount);

    [GlobalCleanup(Target = nameof(Raw))]
    public void SaveRawBytes() =>
        BytesColumn.Save(nameof(ChaosBenchmarks), nameof(Raw), _rawStream!.Length / FrameCount);

    [Benchmark(OperationsPerInvoke = FrameCount)]
    public void Etch()
    {
        _etchStream!.SetLength(0);
        _surface!.Run(new Chaos(), FrameCount);
    }

    [Benchmark(Baseline = true, OperationsPerInvoke = FrameCount)]
    public void Raw()
    {
        _rawStream!.SetLength(0);
        Chaos painter = new();

        for (long frame = 0; frame < FrameCount; frame++)
        {
            painter.Paint(new Context(frame, frame / 144.0, 1f / 144f, _size, _rawWriter!, Span<Color>.Empty));
            _rawWriter!.Flush();
        }
    }
}