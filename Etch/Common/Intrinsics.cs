using System.Numerics;
using System.Runtime.CompilerServices;

namespace Etch.Common;

public static class Intrinsics
{
    /// <summary>
    /// Checks equality after bit casting from <typeparamref name="T"/> to <typeparamref name="U"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals<T, U>(T left, T right)
        where T : struct where U : unmanaged, IEqualityOperators<U, U, bool>, IUnsignedNumber<U>
        => Unsafe.BitCast<T, U>(left) == Unsafe.BitCast<T, U>(right);
}