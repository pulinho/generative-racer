﻿using UnityEngine;

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

    public int GetRowShiftX(int rowIndex) // UnitRowShift?
    {
        // if row > 10
        var heh = (rowIndex / switchAfter) % 6;
        if (heh < 3) return heh - 1;
        return -heh + 4;

        // return ((rowIndex / shitchAfter) % 3) - 1;
    }
}
