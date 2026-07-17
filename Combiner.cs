namespace Etch;

public sealed class Combiner<T, A, B> : IReadOnlyProperty<T>, IDisposable
{
    private readonly IReadOnlyProperty<A> _a;
    private readonly IReadOnlyProperty<B> _b;
    private readonly Func<A, B, T> _combine;

    private T _current;
    private T _previous;
    private bool _dirty;

    public event Action? Changed;
    public Deferrer Deferrer { get; }

    public T Current => _current;
    public T Previous => _previous;
    public T Deferred => _combine(_a.Deferred, _b.Deferred);

    public Combiner(Deferrer deferrer, IReadOnlyProperty<A> a, IReadOnlyProperty<B> b, Func<A, B, T> combine)
    {
        Deferrer = deferrer;

        _a = a;
        _b = b;

        _combine = combine;
        _current = combine(a.Current, b.Current);
        _previous = _current;

        _a.Changed += OnSourceChanged;
        _b.Changed += OnSourceChanged;
    }

    private void OnSourceChanged()
    {
        if (_dirty) return;
        _dirty = true;
        Deferrer.Invalidate(this);
    }

    public void Commit()
    {
        if (!_dirty) return;
        _dirty = false;

        var newValue = _combine(_a.Current, _b.Current);
        if (EqualityComparer<T>.Default.Equals(_current, newValue)) return;

        _previous = _current;
        _current = newValue;
        Changed?.Invoke();
    }

    public void Dispose()
    {
        _a.Changed -= OnSourceChanged;
        _b.Changed -= OnSourceChanged;
    }
}