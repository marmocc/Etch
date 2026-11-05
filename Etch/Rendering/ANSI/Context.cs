using Etch.Common;

namespace Etch.Rendering.ANSI;

public readonly ref struct Context(Arena assets, Surface surface) : IContext
{
    public void FullscreenBlit(Handle handle)
    {
        surface.Cursor = Int2.Zero;
        ReadOnlySpan<Etchel> etchels = assets.Access<Etchel>(handle);
        foreach (var (glyph, foreground, background) in etchels)
        {
            surface.Foreground = foreground;
            surface.Background = background;
            surface.Buffer = glyph;

            SetForeground(foreground);
            SetBackground(background);
            _buffer[_written++] = glyph.Codepoint;
        }
        _cursor = Size - 1;
    }
}
