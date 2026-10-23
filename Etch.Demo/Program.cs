using Etch.Rendering;
using Etch.Rendering.ANSI;
using Etch.Painters;

using Surface surface = new();
Context context = new(surface.Size);
Chaos painter = new();

while(!surface.Stopping)
{
    painter.Paint(context);
    surface.Present(context);
    surface.Flush();
    context.Clear();
}