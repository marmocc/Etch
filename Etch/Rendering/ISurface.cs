namespace Etch.Rendering;

public interface ISurface
{
    void Present(Context context);
    void Flush();
}
