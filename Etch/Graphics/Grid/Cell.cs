namespace Etch.Graphics.Grid;

public readonly struct Cell(Color color, char character)
{
    private static readonly ReadOnlyMemory<char> DensityMap = " .:-=+*#%@".AsMemory();

    public readonly Color Color = color;
    public readonly char Character = character;

    public static Cell Empty => new(Color.Black, ' ');

    public Cell(Color color) : this(color, ComputeDensity(color)) { }

    public static Cell Blend(Cell destination, Cell source)
    {
        if (source.Color.A == 255) return source;
        if (source.Color.A == 0) return destination;

        float alphaFactor = source.Color.A / 255f;
        float invAlphaFactor = 1f - alphaFactor;

        byte r = (byte)(source.Color.R * alphaFactor + destination.Color.R * invAlphaFactor);
        byte g = (byte)(source.Color.G * alphaFactor + destination.Color.G * invAlphaFactor);
        byte b = (byte)(source.Color.B * alphaFactor + destination.Color.B * invAlphaFactor);

        Color blendedColor = new(r, g, b, 255);

        char finalChar = source.Character != ' ' && source.Character != '\0'
            ? source.Character
            : ComputeDensity(blendedColor);

        return new Cell(blendedColor, finalChar);
    }

    private static char ComputeDensity(Color color)
    {
        float luminance = (0.2126f * color.R + 0.7152f * color.G + 0.0722f * color.B) / 255f;
        int mapLength = DensityMap.Length;
        int densityIndex = Math.Clamp((int)(luminance * mapLength), 0, mapLength - 1);
        return DensityMap.Span[densityIndex];
    }
}
