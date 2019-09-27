using UnityEngine;

public interface IHoleMaker
{
    bool MakeHole(int rowIndex, int colIndex, int colCount);
}

public class RandomHoleMaker : IHoleMaker // probability to constructor
{
    public bool MakeHole(int rowIndex, int colIndex, int colCount)
    {
        if (colCount < 3 || colIndex == colCount / 2) // todo handle even colcount; also odd/even rowindex issue
        {
            return false;
        }
        return Random.Range(0, 4) == 0;
    }
}
