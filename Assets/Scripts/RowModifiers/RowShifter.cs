using UnityEngine;

public interface IRowShifter
{
    int GetRowShiftX(int rowIndex); // => return enum {left, none, right}
}

public class RandomRowShifter : IRowShifter // Seed?
{
    public int GetRowShiftX(int rowIndex) // UnitRowShift?
    {
        // if row > 10 => base class ?

        return Random.Range(0, 3) - 1;
    }
}

public class ZigZagRowShifter : IRowShifter
{
    int switchAfter;

    public ZigZagRowShifter(int switchAfter)
    {
        this.switchAfter = switchAfter;
    }

    public int GetRowShiftX(int rowIndex) // UnitaryRowShift?
    {
        var shift = (rowIndex / switchAfter) % 6;
        if (shift < 3) return shift - 1;
        return -shift + 4;
    }
}
