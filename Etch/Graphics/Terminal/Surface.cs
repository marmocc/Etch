using Etch.Geometry;
using System.Buffers;
namespace Etch.Graphics.Terminal;

public sealed class Surface : ISurface<Context>
{
    private readonly ArrayBufferWriter<byte> _writer = new(1024);

    public Stream Stream { get; } = Console.OpenStandardOutput();
    public Vector2<int> Size => new(Console.WindowWidth, Console.WindowHeight);
    public Context Context => new(_writer);

    public Surface()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Clear();

    }

    public void Present()
    {
        Stream.Write(_writer.WrittenSpan);
        _writer.Clear();
    }
}
