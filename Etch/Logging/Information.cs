namespace Etch.Logging;

public enum Information : ushort
{

}

public static partial class Message
{
    public static ReadOnlySpan<byte> For(Information code) => code switch
    {
        _ => throw new NotImplementedException($"{code} has no Message associated to it!"),
    };
}