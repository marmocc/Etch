namespace Etch.Common;

public readonly struct Delta<T>
{
    public readonly T Current;
    public readonly T Previous;
}
