using Etch.Common;

namespace Etch.Rendering.ANSI;

public readonly ref struct Context(Arena assets, Surface surface) : IContext
{
    public Int2 Size => surface.Size;
    public Arena Assets => assets;
    public void Plot(Int2 position, Etchel etchel)
    {
        surface.SetCursor(position);
        surface.SetForeground(etchel.Foreground);
        surface.SetBackground(etchel.Background);
        surface.Put(etchel.Glyph.Codepoint);
        surface.AssumeCursor(position + new Int2(1, 0));
    }
    public void Blit(Handle handle)
    {
        surface.SetCursor(Int2.Zero);
        foreach (var (glyph, fg, bg) in assets.Access<Etchel>(handle))
        {
            surface.SetForeground(fg);
            surface.SetBackground(bg);
            surface.Put(glyph.Codepoint);
        }
        surface.AssumeCursor(surface.Size - 1);
    }
}
