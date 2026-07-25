namespace Etch.Deferred;

public sealed class Property<T> : IReadOnlyProperty<T>, IDisposable
{
    private Action? _unbind;
    
    private T _deferred;
    private bool _dirty;

    private T _current;
    private T _previous;

    public event Action? Changed;
    public Deferrer Deferrer { get; }

    public bool IsBound => _unbind != null;

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
        if (IsBound) throw new InvalidOperationException("Cannot deferredly set a bound Property.");

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

    public void Bind(IReadOnlyProperty<T> source)
    {
        if (IsBound) throw new InvalidOperationException("Cannot bind a bound Property.");

        _dirty = false;
        void OnSourceChanged() => BoundSet(source.Current);
        source.Changed += OnSourceChanged;
        BoundSet(source.Current);
        _unbind = () => {
            source.Changed -= OnSourceChanged;
            _unbind = null;
        };
    }

    public void Unbind()
    {
        if (!IsBound) throw new InvalidOperationException("Cannot unbind an unbound Property.");
        _unbind?.Invoke();
    }

    public void Commit()
    {
        if(!_dirty) return;
        _dirty = false;
        if (EqualityComparer<T>.Default.Equals(_current, _deferred)) return;
        CurrentSet(_deferred);
    }

    public void Dispose() => Unbind();
}