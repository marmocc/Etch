using Etch.Common;
using System.Runtime.InteropServices;

namespace Etch.Graphics;

public readonly struct Delta(Flat index, Pixel pixel)
{
    public readonly Flat Index = index;
    public readonly Pixel Pixel = pixel;

    public static int Compute(ReadOnlySpan<Pixel> first, ReadOnlySpan<Pixel> second, Span<Delta> output)
    {
        int count = 0;
        ReadOnlySpan<uint> firstAsUint = MemoryMarshal.Cast<Pixel, uint>(first);
        ReadOnlySpan<uint> secondAsUint = MemoryMarshal.Cast<Pixel, uint>(second);
        if (firstAsUint.SequenceEqual(secondAsUint)) return count;

        for (int i = 0; i < firstAsUint.Length; i++)
            if (firstAsUint[i] != secondAsUint[i])
                output[count++] = new(new(i), first[i]);

        return count;
    }
}
