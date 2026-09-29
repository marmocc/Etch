using Etch.Common;
using System.Runtime.InteropServices;

namespace Etch.Graphics;

public readonly struct Delta(Flat index, Color color)
{
    public readonly Flat Index = index;
    public readonly Color Color = color;

    public static int Compute(ReadOnlySpan<Color> first, ReadOnlySpan<Color> second, Span<Delta> output)
    {
        int count = 0;
        ReadOnlySpan<uint> firstAsUint = MemoryMarshal.Cast<Color, uint>(first);
        ReadOnlySpan<uint> secondAsUint = MemoryMarshal.Cast<Color, uint>(second);
        if (firstAsUint.SequenceEqual(secondAsUint)) return count;

        for (int i = 0; i < firstAsUint.Length; i++)
            if (firstAsUint[i] != secondAsUint[i])
                output[count++] = new(new(i), first[i]);

        return count;
    }
}
