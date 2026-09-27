using Etch.Common;
using Etch.Terminal;

namespace Etch.Widgets;

public interface IWidget
{
    Int2 Size { get; }
    void Render(Context context);
}
