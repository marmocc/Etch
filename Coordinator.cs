namespace Etch;

public interface ICoordinatable
{
    Coordinator Coordinator { get; }
    public void Deliver();
}

public sealed class Coordinator
{
    private readonly Queue<ICoordinatable> _pendingQueue = new();
    private readonly HashSet<ICoordinatable> _pendingSet = new();

    public static Coordinator Default { get; } = new();
    public Property<T> Property<T>(T value)
    {
        var property = new Property<T>(this, value);
        return property;
    }

    public void Invalidate(ICoordinatable coordinatable)
    {
        if (_pendingSet.Add(coordinatable))
            _pendingQueue.Enqueue(coordinatable);
    }

    public void Flush()
    {
        while (_pendingQueue.TryDequeue(out var coordinatable))
            coordinatable?.Deliver();
        _pendingSet.Clear();
    }
}