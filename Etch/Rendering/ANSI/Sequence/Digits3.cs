using System.Runtime.InteropServices;

namespace Etch.Rendering.ANSI.Sequence;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Digits3
{
    public const int MaxValue = 999;

    public readonly byte Hundreds;
    public readonly byte Tens;
    public readonly byte Ones;

    public Digits3(byte hundreds, byte tens, byte ones)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(hundreds);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(hundreds, 9);

        ArgumentOutOfRangeException.ThrowIfNegative(tens);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(tens, 9);

        ArgumentOutOfRangeException.ThrowIfNegative(ones);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(ones, 9);

        Hundreds = hundreds;
        Tens = tens;
        Ones = ones;
    }

    public Digits3(int value)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxValue);

        Hundreds = (byte)('0' + value / 100);
        Tens = (byte)('0' + value / 10 % 10);
        Ones = (byte)('0' + value % 10);
    }
}