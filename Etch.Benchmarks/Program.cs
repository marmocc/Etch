using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Etch.Backend;
using Etch.Backend.ANSI;
using Etch.Common;
using Etch.Painters;
using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Etch.Benchmarks;

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<EtchAgainstRaw>();
    }
}

[MemoryDiagnoser]
public class EtchAgainstRaw
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
    public void Etch()
    {
        _stream.SetLength(0);
        Chaos painter = new();
        Context context = new(_frame, TimeSpan.Zero, TimeSpan.Zero, _size, _etchels);
        painter.Paint(context);
        _surface.Present(_etchels);
        _frame++;
    }

    [Benchmark(Baseline = true)]
    public void Raw()
    {
        _stream.SetLength(0);

        int totalWritten = 0;
        Span<byte> buffer = stackalloc byte[128];

        // Match Etchel.Solid: Set foreground to Black once for the whole frame
        // \x1b[38;2;0;0;0m
        buffer[0] = 0x1B; buffer[1] = (byte)'['; buffer[2] = (byte)'3'; buffer[3] = (byte)'8';
        buffer[4] = (byte)';'; buffer[5] = (byte)'2'; buffer[6] = (byte)';'; buffer[7] = 0;
        buffer[8] = (byte)';'; buffer[9] = 0; buffer[10] = (byte)';'; buffer[11] = 0;
        buffer[12] = (byte)'m';
        _stream.Write(buffer[..13]);

        for (int y = 0; y < _size.Y; y++)
        {
            // Move Cursor
            int written = 0;
            buffer[written++] = 0x1B; buffer[written++] = (byte)'[';
            Utf8Formatter.TryFormat(y + 1, buffer[written..], out int ry); written += ry;
            buffer[written++] = (byte)';';
            Utf8Formatter.TryFormat(0 + 1, buffer[written..], out int cx); written += cx;
            buffer[written++] = (byte)'H';
            _stream.Write(buffer[..written]);

            for (int x = 0; x < _size.X; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + (long)_frame * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;
                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);

                // Background Only (matching Etchel.Solid)
                written = 0;
                buffer[written++] = 0x1B; buffer[written++] = (byte)'['; buffer[written++] = (byte)'4'; buffer[written++] = (byte)'8'; buffer[written++] = (byte)';'; buffer[written++] = (byte)'2'; buffer[written++] = (byte)';';
                Utf8Formatter.TryFormat(r, buffer[written..], out int r2w); written += r2w;
                buffer[written++] = (byte)';';
                Utf8Formatter.TryFormat(g, buffer[written..], out int g2w); written += g2w;
                buffer[written++] = (byte)';';
                Utf8Formatter.TryFormat(b, buffer[written..], out int b2w); written += b2w;
                buffer[written++] = (byte)'m';
                _stream.Write(buffer[..written]);

                // Glyph
                buffer[0] = (byte)' ';
                _stream.Write(buffer[..1]);
            }
        }
        _frame++;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
        _stream.Dispose();
    }
}
