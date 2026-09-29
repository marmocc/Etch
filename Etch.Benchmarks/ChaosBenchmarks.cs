using BenchmarkDotNet.Attributes;
using Etch.Common;
using Etch.Graphics.Painters;
using Etch.Widgets;
using System.Buffers;
using System.Buffers.Text;

namespace Etch.Benchmarks;

[MemoryDiagnoser]
public class ChaosBenchmarks
{
    [Params(80)] public int Width;
    [Params(40)] public int Height;

    private int _frame;

    private Etcher? _etcher;
    private Canvas? _canvas;
    private MemoryStream? _etchStream;

    private MemoryStream? _rawStream;
    private ArrayBufferWriter<byte>? _rawBuffer;

    [GlobalSetup]
    public void Setup()
    {
        Int2 size = new(Width, Height);
        _frame = 0;

        _etcher = new Etcher();
        _canvas = new Canvas(size, new Chaos());
        _etcher.Add(Int2.Zero, _canvas);
        _etchStream = new MemoryStream(Width * Height * 64);

        _rawStream = new MemoryStream(Width * Height * 64);
        _rawBuffer = new ArrayBufferWriter<byte>(Width * Height * 64);
    }

    [GlobalCleanup(Target = nameof(Etch))]
    public void SaveEtchBytes() =>
        BytesColumn.Save(nameof(ChaosBenchmarks), nameof(Etch), _etchStream!.Length);

    [GlobalCleanup(Target = nameof(Raw))]
    public void SaveRawBytes() =>
        BytesColumn.Save(nameof(ChaosBenchmarks), nameof(Raw), _rawStream!.Length);


    [Benchmark]
    public void Etch()
    {
        _frame++;
        _etchStream!.SetLength(0);
        _etchStream.Position = 0;

        _etcher!.Render(_etchStream);
    }

    [Benchmark(Baseline = true)]
    public void Raw()
    {
        _frame++;
        _rawStream!.SetLength(0);
        _rawStream.Position = 0;

        for (int y = 0; y < Height; y++)
        {
            Move(_rawBuffer!, 0, y);
            for (int x = 0; x < Width; x++)
            {
                Color color = Chaos(x, y);
                Foreground(_rawBuffer!, color);
                Write(_rawBuffer!, Canvas.GlyphFrom(color));
            }
        }

        _rawStream.Write(_rawBuffer!.WrittenSpan);
        _rawBuffer!.Clear();
    }

    private Color Chaos(int x, int y)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + _frame * 2246822519);
        h = (h ^ (h >> 13)) * 1274126177;
        h ^= h >> 16;

        byte r = (byte)(h);
        byte g = (byte)(h >> 8);
        byte b = (byte)(h >> 16);
        return new Color(r, g, b, 255);
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
