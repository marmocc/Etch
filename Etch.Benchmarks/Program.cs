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

/// <summary>
/// A stream that ignores all writes, preventing "Stream too long" errors during benchmarks.
/// </summary>
public sealed class NullStream : Stream
{
    public override bool CanRead => false;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => 0;
    public override long Position { get => 0; set { } }
    public override void Flush() { }
    public override int Read(Span<byte> buffer) => 0;
    public override void Write(ReadOnlySpan<byte> buffer) { }
    public override void SetLength(long value) { }
    public override int ReadByte() => -1;
    public override void WriteByte(byte value) { }
}

[MemoryDiagnoser]
public class RenderLoopBenchmarks
{
    private Surface _surface = default!;
    private Stream _stream = default!;
    private Etchel[] _etchels = default!;
    private Int2 _size = new(80, 40);

    [GlobalSetup]
    public void Setup()
    {
        _stream = new NullStream();
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

    private ulong _frame = 0;

    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
        _stream.Dispose();
    }
}
