using Etch.Rendering;
using Etch.Rendering.ANSI;
using Etch.Painters;

Surface surface = new();
Context context = new(surface.Size);
Chaos spiral = new();

while(!surface.Stopping)
{
    spiral.Paint(context);
    surface.Dispatch(context);
    surface.Present();
    context.Clear();
}