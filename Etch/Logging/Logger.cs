using Etch.Common;

namespace Etch.Logging;

public sealed class Logger(int maxBufferCapacity)
{
    public Code ThrowOn { get; }

    public readonly Ring<float> FrametimeBuffer = new(maxBufferCapacity);
    public readonly Ring<Code> CodeBuffer = new(maxBufferCapacity);

    public void Log(float frametime) => FrametimeBuffer.Push(frametime);
    public void Log(Code code)
    {
        if(ThrowOn == code) throw new Exception($"ThrowOn {code} condition was triggered.");
        CodeBuffer.Push(code);
    }
}