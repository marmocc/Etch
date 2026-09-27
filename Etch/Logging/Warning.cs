namespace Etch.Logging;

public enum Warning : ushort
{
    PlotOutOfBounds,
    BlitOutOfBounds,
}

public static partial class Message
{
    public static ReadOnlySpan<byte> For(Warning code) => code switch
    {
        _ => throw new NotImplementedException($"{code} has no Message associated to it!"),
    };
}