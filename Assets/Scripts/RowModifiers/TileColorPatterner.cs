using UnityEngine;

public interface ITileColorPatterner
{
    Color GetTileColor(int row, int col, int colShift); // => return enum {left, none, right}
}

public class RandomTileColorPatterner : ITileColorPatterner
{
    public Color GetTileColor(int row, int col, int colShift)
    {
        return Random.ColorHSV();
    }
}

public class GrayWhiteTileColorPatterner : ITileColorPatterner
{
    public Color GetTileColor(int row, int col, int colShift)
    {
        var gray = (row + col + colShift) % 2 == 0;
        return gray ? Color.grey : Color.white;
    }
}
