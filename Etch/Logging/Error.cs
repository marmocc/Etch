namespace Etch.Logging;

public enum Error : ushort
{

}

public static partial class Message
{
    public static ReadOnlySpan<byte> For(Error code) => code switch
    {
        _ => throw new NotImplementedException($"{code} has no Message associated to it!"),
    };
}