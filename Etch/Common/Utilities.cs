using System.Runtime.CompilerServices;

namespace Etch.Common;

public static class Utilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte Div255(int x) => (byte)((x + 1 + (x >> 8)) >> 8);
}
