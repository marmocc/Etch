using System.Runtime.InteropServices;

namespace Etch.Common;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Digit3
{
    public const int MaxValue = 999;

    public readonly byte Hundreds;
    public readonly byte Tens;
    public readonly byte Ones;

    public Digit3(byte value)
    {
        Hundreds = (byte)('0' + value / 100);
        Tens = (byte)('0' + value / 10 % 10);
        Ones = (byte)('0' + value % 10);
    }

    public Digit3(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxValue);

        Hundreds = (byte)('0' + value / 100);
        Tens = (byte)('0' + value / 10 % 10);
        Ones = (byte)('0' + value % 10);
    }
}