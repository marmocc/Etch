using Etch.Graphics;

namespace Etch.Backend;

internal interface ISurface
{
    void Present(Canvas canvas);
}
