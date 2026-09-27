namespace Etch.Logging;

public static class Message
{
    public static ReadOnlySpan<byte> For(Code code) => code switch
    {
        _ => throw new NotImplementedException($"{code} has no Message associated to it!"),
    };
}