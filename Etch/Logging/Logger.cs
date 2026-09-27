using Etch.Common;
using System.Text;

namespace Etch.Logging;

public sealed class Logger(int maxInformationCount, int maxWarningCount, int maxErrorCount)
{
    public bool ThrowOnInformation = false;
    public bool ThrowOnWarning = false;
    public bool ThrowOnError = false;

    public readonly Ring<Information> InformationBuffer = new(maxInformationCount);
    public readonly Ring<Warning> WarningBuffer = new(maxWarningCount);
    public readonly Ring<Error> ErrorBuffer = new(maxErrorCount);

    public void Log(Information code)
    {
        if (ThrowOnInformation) throw new Exception($"[{code}] {Encoding.UTF8.GetString(Message.For(code))}");
        InformationBuffer.Push(code);
    }

    public void Log(Warning code)
    {
        if (ThrowOnWarning) throw new Exception($"[{code}] {Encoding.UTF8.GetString(Message.For(code))}");
        WarningBuffer.Push(code);
    }

    public void Log(Error code)
    {
        if (ThrowOnError) throw new Exception($"[{code}] {Encoding.UTF8.GetString(Message.For(code))}");
        ErrorBuffer.Push(code);
    }
}
