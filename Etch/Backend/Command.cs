using System.Runtime.CompilerServices;

namespace Etch.Backend;

public interface ICommand 
{ 
    byte Tag { get; } 
    void Execute();
}

public readonly record struct Tagged(byte Tag)
{
    private readonly byte _byte1, _byte2, _byte3, _byte4, _byte5, _byte6, _byte7;
    public static Tagged From<T>(T command) where T : ICommand => Unsafe.BitCast<T, Tagged>(command);
    public T To<T>() where T : ICommand => Unsafe.BitCast<Tagged, T>(this);
}
public readonly record struct Generic(ulong Packed) : IEquatable<Generic>
{
    public static Generic From<T>(T command) where T : ICommand => Unsafe.BitCast<T, Generic>(command);
    public T To<T>() where T : ICommand => Unsafe.BitCast<Generic, T>(this);

    public bool Equals(Generic other) => Packed == other.Packed;
    public override int GetHashCode() => Packed.GetHashCode();
}

public readonly record struct Etchels(byte Tag, int Offset, int Length) : ICommand
{
    public void Execute()
    {

    }
}