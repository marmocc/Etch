using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Etch.Common;

/// <summary>
/// Defines a Field that's packed inside a <typeparamref name="T"/> and provides accessor methods.
/// </summary>
/// <typeparam name="T">Type of the backing field.</typeparam>
public readonly struct Field<T>
    where T : unmanaged, IBinaryInteger<T>, IUnsignedNumber<T>
{
    public readonly int Shift;
    public readonly T Mask;

    public Field(int shift, int size)
    {
        int bits = Unsafe.SizeOf<T>() * 8;
        ArgumentOutOfRangeException.ThrowIfLessThan(size, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(shift, 0);
        if (shift + size > bits) throw new ArgumentOutOfRangeException(nameof(size), "Field does not fit in the storage type.");

        Shift = shift;
        Mask = T.AllBitsSet >> (bits - size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Read(T storage) => (storage >> Shift) & Mask;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Write(T storage, T value)
    {
        Debug.Assert((value & ~Mask) == T.Zero, "Value does not fit in the field.");
        return (storage & ~(Mask << Shift)) | ((value & Mask) << Shift);
    }
}