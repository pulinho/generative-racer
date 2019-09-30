using UnityEngine;

public interface IColorPalette
{
    Color[] Get2Colors(int row);
}

public class ColorPalette : IColorPalette
{
    public Color[] Get2Colors(int row)
    {
        var hue = (row % 77) / 77f;
        var hue2 = (hue + 0.5f) % 1f;

        return new Color[]
        {
            Color.HSVToRGB(hue, 0.8f, 1f),
            Color.HSVToRGB(hue2, 0.8f, 1f)
        };
    }
}
