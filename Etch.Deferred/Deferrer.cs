namespace Etch.Deferred;

public sealed class Deferrer
{
    private readonly Queue<IDeferrable> _pendingQueue = [];
    private readonly HashSet<IDeferrable> _pendingSet = [];

    public Property<T> Property<T>(T value)
    {
        var property = new Property<T>(this, value);
        return property;
    }

    public void Invalidate(IDeferrable deferrable)
    {
        if (_pendingSet.Add(deferrable))
            _pendingQueue.Enqueue(deferrable);
    }

    public void Flush()
    {
        while (_pendingQueue.TryDequeue(out var deferrable))
            deferrable?.Commit();
        _pendingSet.Clear();
    }
}