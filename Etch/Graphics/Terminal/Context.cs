using System.Buffers;
using System.Text;

namespace Etch.Graphics.Terminal;

public readonly ref struct Context(ArrayBufferWriter<byte> writer) : IContext
{
    private readonly ArrayBufferWriter<byte> _writer = writer;

    public void Clear(Color fill)
    {
        ANSI.Color(_writer, fill, false);
        ANSI.Clear(_writer);
    }

    public void Plot(Vector2<int> position, Color color)
    {
        ANSI.Color(_writer, color, true);
        ANSI.Move(_writer, position.Y, position.X);
        Encoding.UTF8.GetBytes("█", _writer);
    }

    public void Draw(Vector2<int> position, Vector2<int> size, Color color)
    {
        ANSI.Color(_writer, color, true);
        ANSI.Move(_writer, position.Y, position.X);
    }

    public void Write(Vector2<int> position, ReadOnlySpan<char> text, Color foreground, Color background)
    {
        ANSI.Color(_writer, background, false);
        ANSI.Color(_writer, foreground, true);
        ANSI.Move(_writer, position.Y, position.X);
        Encoding.UTF8.GetBytes(text, _writer);
    }
}