using System.Runtime.CompilerServices;

namespace Etch.Backend;

public static class Commands
{
    public interface ICommand { byte Tag { get; } }
    public readonly record struct Generic(byte Tag) : ICommand, IEquatable<Generic>
    {
        private readonly byte _byte1, _byte2, _byte3, _byte4, _byte5, _byte6, _byte7;

        public ulong Packed => Unsafe.BitCast<Generic, ulong>(this);
        public static Generic From<T>(T command) where T : ICommand => Unsafe.BitCast<T, Generic>(command);
        public T To<T>() where T : ICommand => Unsafe.BitCast<Generic, T>(this);

        public bool Equals(Generic other) => Packed == other.Packed;
        public override int GetHashCode() => Packed.GetHashCode();
    }

    public readonly record struct Etchels(byte Tag, int Offset, int Length) : ICommand
    {

    }
}

public static class Executors
{
    public interface IExecutor<T> where T : Commands.ICommand
    {
        public void Execute(T command);
    }

    public readonly ref struct EtchelsOnANSI : IExecutor<Commands.Etchels>
    {
        public void Execute(Commands.Etchels command)
        {
            // ANSI implementation of the Etchels command
        }
    }
}

public static class Dispatcher
{
    public static void Dispatch(Commands.Generic command)
    {
        switch (command.Tag)
        {
            case 0: new Executors.EtchelsOnANSI().Execute(command.To<Commands.Etchels>()); break;
            default: throw new NotImplementedException();
        }
    }
}