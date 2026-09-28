using BenchmarkDotNet.Attributes;
using Etch.Common;
using Etch.Graphics.Painters;
using Etch.Widgets;
using System.Buffers;
using System.Buffers.Text;

namespace Etch.Benchmarks;

[MemoryDiagnoser]
public class SpiralBenchmarks
{
    [Params(80)] public int Width;
    [Params(40)] public int Height;

    private float _centerX, _centerY, _aspect;
    private const float FakeDeltaTime = 1f / 144f;
    private float _time;


    private Etcher? _etcher;
    private Canvas? _canvas;
    private MemoryStream? _etchStream;

    private MemoryStream? _rawStream;
    private ArrayBufferWriter<byte>? _rawBuffer;

    [GlobalSetup]
    public void Setup()
    {
        Int2 size = new(Width, Height);
        _centerX = Width / 2.0f;
        _centerY = Height / 2.0f;
        _aspect = Width > 0 ? (float)Height / Width : 1.0f;
        _time = 0;

        _etcher = new Etcher();
        _canvas = new Canvas(size, new Spiral());
        _etcher.Add(Int2.Zero, _canvas);
        _etchStream = new MemoryStream(Width * Height * 64);

        _rawStream = new MemoryStream(Width * Height * 64);
        _rawBuffer = new ArrayBufferWriter<byte>(Width * Height * 64);
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

        _etcher!.Render(_etchStream);
    }

    [Benchmark(Baseline = true)]
    public void Raw()
    {
        _time += FakeDeltaTime;
        _rawStream!.SetLength(0);
        _rawStream.Position = 0;

        for (int y = 0; y < Height; y++)
        {
            Move(_rawBuffer!, 0, y);
            for (int x = 0; x < Width; x++)
            {
                Color color = Spiral(x, y, _time);
                Foreground(_rawBuffer!, color);
                Write(_rawBuffer!, color.Glyph);
            }
        }

        _rawStream.Write(_rawBuffer!.WrittenSpan);
        _rawBuffer!.Clear();
    }

    private Color Spiral(int x, int y, float time)
    {
        float dx = (x - _centerX);
        float dy = (y - _centerY) / _aspect;

        float angle = MathF.Atan2(dy, dx);
        float radius = MathF.Sqrt(dx * dx + dy * dy);

        float n = MathF.Sin(radius * 0.5f - time * 3f + angle * 4f);
        byte v = (byte)((n + 1) / 2 * 255);

        return new Color(v, (byte)(v / 3), (byte)(255 - v), 255);
    }

    private static void Write(ArrayBufferWriter<byte> _output, byte glyph)
    {
        Span<byte> buffer = _output.GetSpan(1);
        buffer[0] = glyph;
        _output.Advance(1);
    }
    private static void Move(ArrayBufferWriter<byte> _output, int x, int y)
    {
        Span<byte> buffer = _output.GetSpan(16);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';

        bool rowSuccess = Utf8Formatter.TryFormat(y + 1, buffer[written..], out int rowWritten);
        if (!rowSuccess) throw new InvalidOperationException("Failed to format row value.");
        written += rowWritten;

        buffer[written++] = (byte)';';

        bool colSuccess = Utf8Formatter.TryFormat(x + 1, buffer[written..], out int colWritten);
        if (!colSuccess) throw new InvalidOperationException("Failed to format column value.");
        written += colWritten;

        buffer[written++] = (byte)'H';
        _output.Advance(written);
    }
    private static void Foreground(ArrayBufferWriter<byte> _output, Color foreground)
    {
        Span<byte> buffer = _output.GetSpan(24);
        int written = 0;

        buffer[written++] = 0x1B; // ESC
        buffer[written++] = (byte)'[';
        buffer[written++] = (byte)'3'; // 3 for Foreground
        buffer[written++] = (byte)'8';
        buffer[written++] = (byte)';';
        buffer[written++] = (byte)'2';
        buffer[written++] = (byte)';';

        bool rSuccess = Utf8Formatter.TryFormat(foreground.R, buffer[written..], out int rWritten);
        if (!rSuccess) throw new InvalidOperationException("Failed to format red value.");
        written += rWritten;

        buffer[written++] = (byte)';';

        bool gSuccess = Utf8Formatter.TryFormat(foreground.G, buffer[written..], out int gWritten);
        if (!gSuccess) throw new InvalidOperationException("Failed to format green value.");
        written += gWritten;

        buffer[written++] = (byte)';';

        bool bSuccess = Utf8Formatter.TryFormat(foreground.B, buffer[written..], out int bWritten);
        if (!bSuccess) throw new InvalidOperationException("Failed to format blue value.");
        written += bWritten;

        buffer[written++] = (byte)'m';
        _output.Advance(written);
    }
}
