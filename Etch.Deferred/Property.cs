namespace Etch.Deferred;

public interface IReadOnlyProperty<T> : IDeferrable
{
    event Action? Changed;
    T Current { get; }
    T Previous { get; }
    T Deferred { get; }
}

public sealed class Property<T> : IReadOnlyProperty<T>
{
    private bool _bound;
    
    private T _deferred;
    private bool _dirty;

    private T _current;
    private T _previous;

    public event Action? Changed;
    public Deferrer Deferrer { get; }

    public bool IsBound => _bound;

    public T Current => _current;
    public T Previous => _previous;
    public T Deferred
    {
        get => _deferred;
        set => DeferredSet(value);
    }

    internal Property(Deferrer deferrer, T value)
    {
        Deferrer = deferrer;
        _deferred = value;
        _current = value;
        _previous = value;
    }

    private void DeferredSet(T value)
    {
        if (_bound) throw new InvalidOperationException("Cannot deferredly set a bound Property.");

        if (EqualityComparer<T>.Default.Equals(_deferred, value)) return;
        _deferred = value;

        if (_dirty) return;
        _dirty = true;
        Deferrer.Invalidate(this);
    }

    private void CurrentSet(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_current, value)) return;
        _previous = _current;
        _current = value;
        Changed?.Invoke();
    }

    private void BoundSet(T value)
    {
        CurrentSet(value);
        _deferred = _current;
    }

    public Cleanup Bind(IReadOnlyProperty<T> source)
    {
        if (_bound) throw new InvalidOperationException("Cannot bind a bound Property.");

        _bound = true;
        _dirty = false;
        void OnSourceChanged() => BoundSet(source.Current);
        source.Changed += OnSourceChanged;
        BoundSet(source.Current);

        return new Cleanup(() => {
            source.Changed -= OnSourceChanged;
            _bound = false;
        });
    }

    public void Commit()
    {
        if(!_dirty) return;
        _dirty = false;
        if (EqualityComparer<T>.Default.Equals(_current, _deferred)) return;
        CurrentSet(_deferred);
    }
}