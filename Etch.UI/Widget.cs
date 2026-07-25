using Etch.Deferred;
using Etch.Platform;
using Etch.Primitives;

namespace Etch.UI;

public class Widget : IWidget
{
    private readonly Property<Float2> _position;
    private readonly Property<Float2> _size;
    private readonly Property<int> _zIndex;
    private readonly Property<Color> _color;

    public IReadOnlyProperty<Float2> Position => _position;
    public IReadOnlyProperty<Float2> Size => _size;
    public IReadOnlyProperty<int> ZIndex => _zIndex;
    public IReadOnlyProperty<Color> Color => _color;

    public event Action? Invalidated;

    public Widget(Deferrer deferrer, Float2 position = default, Float2 size = default, int zIndex = 0, Color color = default)
    {
        _position = deferrer.Property(position);
        _size = deferrer.Property(size);
        _zIndex = deferrer.Property(zIndex);
        _color = deferrer.Property(color);

        _position.Changed += OnPropertyChanged;
        _size.Changed += OnPropertyChanged;
        _zIndex.Changed += OnPropertyChanged;
        _color.Changed += OnPropertyChanged;
    }

    protected void OnPropertyChanged() => Invalidated?.Invoke();

    public void Render<TContext>(TContext context) where TContext : IContext, allows ref struct
    {
        var rect = new Rect(Position.Current, Size.Current);
        context.FillRect(rect, Color.Current);
    }
}
