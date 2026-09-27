using System.Runtime.CompilerServices;

namespace Etch.Common;

public static class Utilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte DivisionBy255(int x) => (byte)((x + 1 + (x >> 8)) >> 8);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int CeilingDivision(int x, int y) => (x + y - 1) / y;
}
