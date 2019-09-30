using UnityEngine;

public interface IGridColorPatterner
{
    Color GetTileColor(int row, int col, int colShift); // => return enum {left, none, right}
}

public abstract class TwoColorGridPatterner : IGridColorPatterner
{
    public Color[] colors = new Color[] { Color.black, Color.white };

    public abstract Color GetTileColor(int row, int col, int colShift);
}

public class RandomGridColorPatterner : IGridColorPatterner
{
    public Color GetTileColor(int row, int col, int colShift)
    {
        // return Random.ColorHSV();
        var type = row % 4;
        return Random.ColorHSV(type * 0.25f, type * 0.25f + 0.05f, 0.1f, 0.2f, 0.9f, 1, 1, 1);
    }
}

public class ChessGridColorPatterner : TwoColorGridPatterner
{
    public override Color GetTileColor(int row, int col, int colShift)
    {
        var black = (row + col + colShift) % 2 == 0;
        return black ? colors[0] : colors[1];
    }
}

public class ChessGridNoShiftColorPatterner : TwoColorGridPatterner
{
    public override Color GetTileColor(int row, int col, int colShift)
    {
        var black = (row + col) % 2 == 0;
        return black ? colors[0] : colors[1];
    }
}

public class StripesColorPatterner : TwoColorGridPatterner
{
    public override Color GetTileColor(int row, int col, int colShift)
    {
        var black = (colShift + col) % 2 == 0;
        return black ? colors[0] : colors[1];
    }
}

public class StripesNoShiftPatterner : TwoColorGridPatterner
{
    public override Color GetTileColor(int row, int col, int colShift)
    {
        var black = col % 2 == 0;
        return black ? colors[0] : colors[1];
    }
}
