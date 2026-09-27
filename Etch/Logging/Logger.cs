using Etch.Common;
using System.Text;

namespace Etch.Logging;

public sealed class Logger(int maxInformationCount, int maxWarningCount, int maxErrorCount)
{
    public bool ThrowOnInformation = false;
    public bool ThrowOnWarning = false;
    public bool ThrowOnError = false;

    private readonly Ring<Information> _informationBuffer = new(maxInformationCount);
    private readonly Ring<Warning> _warningBuffer = new(maxWarningCount);
    private readonly Ring<Error> _errorBuffer = new(maxErrorCount);

    public void Log(Information code)
    {
        if (ThrowOnInformation) throw new Exception($"[{code}] {Encoding.UTF8.GetString(Message.For(code))}");
        _informationBuffer.Push(code);
    }

    public void Log(Warning code)
    {
        if (ThrowOnWarning) throw new Exception($"[{code}] {Encoding.UTF8.GetString(Message.For(code))}");
        _warningBuffer.Push(code);
    }

    public void Log(Error code)
    {
        if (ThrowOnError) throw new Exception($"[{code}] {Encoding.UTF8.GetString(Message.For(code))}");
        _errorBuffer.Push(code);
    }
}
