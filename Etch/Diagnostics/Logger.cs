using Etch.Common;

namespace Etch.Diagnostics;

public sealed class Logger(int maxBufferCapacity)
{
    public List<Code> ThrowOn { get; } = [];
    public readonly Ring<Code> Codes = new(maxBufferCapacity);

    public void Log(Code code)
    {
        Codes.Push(code);
        if(ThrowOn.Contains(code)) throw new Exception($"ThrowOn was triggered with {code}.");
    }
}