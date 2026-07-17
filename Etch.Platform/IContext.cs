namespace Etch.Platform;

public interface IContext
{
    void Clear(Color color);
    void FillRect(Rect rect, Color color);
}