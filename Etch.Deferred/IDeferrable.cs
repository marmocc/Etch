namespace Etch.Deferred;

public interface IDeferrable
{
    Deferrer Deferrer { get; }
    void Commit();
}
