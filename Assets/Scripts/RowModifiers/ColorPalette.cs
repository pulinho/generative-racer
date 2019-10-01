using UnityEngine;

public interface IColor2Palette
{
    Color[] Get2Colors(int row);
}

public class Color2Palette : IColor2Palette
{
    public Color[] Get2Colors(int row)
    {
        var hue = (row % 77) / 77f;
        var hue2 = (hue + 0.5f) % 1f;

        return new Color[]
        {
            Color.HSVToRGB(hue, 0.8f, 1f),
            Color.HSVToRGB(hue2, 0.6f, 0.75f)
        };
    }
}

public interface IColor3Palette
{
    Color[] Get3Colors(int row);
}

public class Color3Palette : IColor3Palette
{
    public Color[] Get3Colors(int row)
    {
        var hue = (row % 77) / 77f;
        var hue2 = (hue + 0.333f) % 1f;
        var hue3 = (hue + 0.666f) % 1f;

        return new Color[]
        {
            Color.HSVToRGB(hue, 0.8f, 1f),
            Color.HSVToRGB(hue2, 0.6f, 0.5f),
            Color.HSVToRGB(hue3, 0.4f, 0.75f)
        };
    }
}
