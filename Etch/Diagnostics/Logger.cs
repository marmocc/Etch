using Etch.Common;

namespace Etch.Diagnostics;

public sealed class Logger(int maxBufferCapacity)
{
    public List<Code> ThrowOn { get; } = [];

    public readonly Ring<float> FrametimeBuffer = new(maxBufferCapacity);
    public readonly Ring<Code> CodeBuffer = new(maxBufferCapacity);

    public void Log(float frametime) => FrametimeBuffer.Push(frametime);
    public void Log(Code code)
    {
        if(ThrowOn.Contains(code)) throw new Exception($"ThrowOn was triggered with {code}.");
        CodeBuffer.Push(code);
    }
}