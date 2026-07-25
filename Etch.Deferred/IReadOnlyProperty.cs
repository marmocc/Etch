namespace Etch.Deferred;

public interface IReadOnlyProperty<T> : IDeferrable
{
    event Action? Changed;
    T Current { get; }
    T Previous { get; }
    T Deferred { get; }
}