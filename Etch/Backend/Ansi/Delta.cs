using Etch.Common;
using Etch.Graphics;
using System.Runtime.InteropServices;

namespace Etch.Backend.Ansi;

public readonly struct Delta(Flat index, Color color)
{
    public readonly Flat Index = index;
    public readonly Color Color = color;

    public static int Compute(ReadOnlySpan<Color> newSpan, ReadOnlySpan<Color> oldSpan, Span<Delta> output)
    {
        int count = 0;
        ReadOnlySpan<uint> newAsUint = MemoryMarshal.Cast<Color, uint>(newSpan);
        ReadOnlySpan<uint> oldAsUint = MemoryMarshal.Cast<Color, uint>(oldSpan);
        if (newAsUint.SequenceEqual(oldAsUint)) return count;

        for (int i = 0; i < newAsUint.Length; i++)
            if (newAsUint[i] != oldAsUint[i])
                output[count++] = new(new(i), newSpan[i]);

        return count;
    }
}
