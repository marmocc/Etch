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

    [Benchmark]
    public void Raw()
    {
        _stream.SetLength(0);

        // We simulate the Chaos painter logic but write directly to the stream
        // bypassing Etchel, Context, and Surface diffing.
        // We still use the terminal state optimizations (only move/color if changed).

        // For Raw, we can't easily maintain a "shadow" state without adding more fields
        // so we just do a full screen write which is what Etch does when every cell changes.

        Span<byte> buffer = stackalloc byte[64];
        int written = 0;

        for (int y = 0; y < _size.Y; y++)
        {
            // Move Cursor
            written = 0;
            buffer[written++] = 0x1B; buffer[written++] = (byte)'[';
            Utf8Formatter.TryFormat(y + 1, buffer[written..], out int ry); written += ry;
            buffer[written++] = (byte)';';
            Utf8Formatter.TryFormat(0 + 1, buffer[written..], out int cx); written += cx; // Start of line
            buffer[written++] = (byte)'H';
            _stream.Write(buffer[..written]);

            for (int x = 0; x < _size.X; x++)
            {
                // Chaos Math
                uint h = (uint)(x * 374761393 + y * 668265263 + (long)_frame * 2246822519);
                h = (h ^ (h >> 13)) * 1274126177;
                h ^= h >> 16;
                byte r = (byte)(h);
                byte g = (byte)(h >> 8);
                byte b = (byte)(h >> 16);

                // Foreground
                written = 0;
                buffer[written++] = 0x1B; buffer[written++] = (byte)'['; buffer[written++] = (byte)'3'; buffer[written++] = (byte)'8'; buffer[written++] = (byte)';'; buffer[written++] = (byte)'2'; buffer[written++] = (byte)';';
                Utf8Formatter.TryFormat(r, buffer[written..], out int rw); written += rw;
                buffer[written++] = (byte)';';
                Utf8Formatter.TryFormat(g, buffer[written..], out int gw); written += gw;
                buffer[written++] = (byte)';';
                Utf8Formatter.TryFormat(b, buffer[written..], out int bw); written += bw;
                buffer[written++] = (byte)'m';
                _stream.Write(buffer[..written]);

                // Background
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
                buffer[0] = (byte)' '; // Chaos uses Space for Solid
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
