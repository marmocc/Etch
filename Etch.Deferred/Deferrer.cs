namespace Etch.Deferred;

public interface IDeferrable
{
    Deferrer Deferrer { get; }
    public void Commit();
}

public sealed class Deferrer
{
    private readonly Queue<IDeferrable> _pendingQueue = new();
    private readonly HashSet<IDeferrable> _pendingSet = new();

    public Property<T> Property<T>(T value)
    {
        var property = new Property<T>(this, value);
        return property;
    }

    public void Invalidate(IDeferrable coordinatable)
    {
        if (_pendingSet.Add(coordinatable))
            _pendingQueue.Enqueue(coordinatable);
    }

    public void Flush()
    {
        while (_pendingQueue.TryDequeue(out var deferrable))
            deferrable?.Commit();
        _pendingSet.Clear();
    }
}