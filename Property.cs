namespace Etch;

public interface IReadOnlyProperty<T> : ICoordinatable
{
    event Action<T, T>? Changed;
    T Value { get; }
}

public sealed class Property<T> : IReadOnlyProperty<T>
{
    private T _value;
    private T _pending;
    private bool _dirty;

    public event Action<T, T>? Changed;

    public Coordinator Coordinator { get; }
    public T Value
    {
        get { return _value; }
        set { Set(value); }
    }

    internal Property(Coordinator coordinator, T value)
    {
        Coordinator = coordinator;

        _pending = default!;
        _value = value;
    }

    private void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;

        if (!_dirty)
        {
            _pending = _value;
            _dirty = true;
            Coordinator.Invalidate(this);
        }

        _value = value;
    }

    public void Deliver()
    {
        if(!_dirty) return;
        _dirty = false;
        if(!EqualityComparer<T>.Default.Equals(_pending, _value))
            Changed?.Invoke(_pending, _value);
    }
}