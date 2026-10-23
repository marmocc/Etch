using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Etch.Rendering;
using Etch.Rendering.ANSI;
using Etch.Common;
using Etch.Painters;
using System.Buffers.Text;

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
    private Chaos _painter = default!;
    private Context _context = default!;
    private Surface _surface = default!;
    private MemoryStream _stream = default!;
    private byte[] _rawBuffer = default!;
    private Int2 _size = new(80, 40);
    private ulong _frame = 0;

    [GlobalSetup]
    public void Setup()
    {
        _stream = new MemoryStream();
        _painter = new Chaos();
        _context = new Context(_size);
        _surface = new Surface(_size, _stream);
        _rawBuffer = new byte[1024 * 256];
    }

    [Benchmark]
    public void Etch()
    {
        _stream.SetLength(0);
        _painter.Paint(_context);
        _surface.Present(_context);
        _context.Clear();
    }

    [Benchmark(Baseline = true)]
    public void Raw()
    {
        _stream.SetLength(0);

        int totalWritten = 0;
        Span<byte> buffer = _rawBuffer;

        // Match Etchel.Solid: Set foreground to Black once for the whole frame
        // \x1b[38;2;0;0;0m
        buffer[totalWritten++] = 0x1B; buffer[totalWritten++] = (byte)'['; buffer[totalWritten++] = (byte)'3'; buffer[totalWritten++] = (byte)'8';
        buffer[totalWritten++] = (byte)';'; buffer[totalWritten++] = (byte)'2'; buffer[totalWritten++] = (byte)';'; buffer[totalWritten++] = 0;
        buffer[totalWritten++] = (byte)';'; buffer[totalWritten++] = 0; buffer[totalWritten++] = (byte)';'; buffer[totalWritten++] = 0;
        buffer[totalWritten++] = (byte)'m';

        for (int y = 0; y < _size.Y; y++)
        {
            // Move Cursor
            buffer[totalWritten++] = 0x1B; buffer[totalWritten++] = (byte)'[';
            Utf8Formatter.TryFormat(y + 1, buffer.Slice(totalWritten), out int ry); totalWritten += ry;
            buffer[totalWritten++] = (byte)';';
            Utf8Formatter.TryFormat(0 + 1, buffer.Slice(totalWritten), out int cx); totalWritten += cx;
            buffer[totalWritten++] = (byte)'H';

            for (int x = 0; x < _size.X; x++)
            {
                uint h = (uint)(x * 374761393 + y * 668265263 + (long)_frame * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;
                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);

                // Background Only (matching Etchel.Solid)
                buffer[totalWritten++] = 0x1B; buffer[totalWritten++] = (byte)'['; buffer[totalWritten++] = (byte)'4'; buffer[totalWritten++] = (byte)'8'; buffer[totalWritten++] = (byte)';'; buffer[totalWritten++] = (byte)'2'; buffer[totalWritten++] = (byte)';';
                Utf8Formatter.TryFormat(r, buffer.Slice(totalWritten), out int rw); totalWritten += rw;
                buffer[totalWritten++] = (byte)';';
                Utf8Formatter.TryFormat(g, buffer.Slice(totalWritten), out int gw); totalWritten += gw;
                buffer[totalWritten++] = (byte)';';
                Utf8Formatter.TryFormat(b, buffer.Slice(totalWritten), out int bw); totalWritten += bw;
                buffer[totalWritten++] = (byte)'m';

                // Glyph
                buffer[totalWritten++] = (byte)' ';
            }
        }

        _stream.Write(_rawBuffer, 0, totalWritten);
        _frame++;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _surface.Dispose();
        _stream.Dispose();
    }
}
