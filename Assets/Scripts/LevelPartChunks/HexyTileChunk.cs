using UnityEngine;
using System.Collections;

// inyerface?

public interface IRowShifter
{
    int GetRowShift(int rowIndex); // => return enum {left, none, right}
}

public class RandomRowShifter : IRowShifter // Seed?
{
    public int GetRowShift(int rowIndex)
    {
        return Random.Range(0, 3) - 1;
    }
}

public class HexyTileChunk : MonoBehaviour // HexTilePathGenerator
{
    public GameObject pointTopPrefab;
    public GameObject flatTopPrefab;

    public bool isPointTop;
    public bool endsWithCurve; // decorator
    public bool endCurveGoesLeft; // also?

    public int rowCount;
    public int colCount;

    public float sideSize;

    float chunkWidth;
    float colWidth;
    float colHeight;

    /*public*/ IRowShifter rowShifter = new RandomRowShifter();

    int shifterShiftX; // NEBO Vector3 currentDrawingPos => to by ovsem bylo na GetNextRow misto GetRow(int rowIndex)

    void Awake()
    {
        colWidth = isPointTop
            ? sideSize * Mathf.Sqrt(3f)
            : sideSize * 1.5f;
        colHeight = isPointTop
            ? sideSize * 1.5f
            : sideSize * Mathf.Sqrt(3f);
        chunkWidth = colWidth * colCount;
    }

    public Vector3 GetWorldExitPosition() // screw this I guess? Just generate row after row. Wattabout curves tho...
    {
        //shiftDecorator

        return transform.TransformPoint(new Vector3(
            0f,
            0f,
            rowCount * colHeight));
    }

    public Quaternion GetWorldExitRotation()
    {
        return Quaternion.identity;
    }

    public GameObject[] GetRow(int rowIndex)
    {
        var tileRow = new GameObject[colCount];

        shifterShiftX += rowShifter?.GetRowShift(rowIndex) ?? 0; // shift by col size on X axis

        if (isPointTop) // separate classes?
        {
            float zigZagShiftX = ((rowIndex % 2 == 0) ?  -1 : 1) * (colWidth / 4f);
            int localRowIndex = rowIndex % rowCount;

            for (int i = 0; i < colCount; i++)
            {
                var position = new Vector3(
                    -chunkWidth / 2f + i * colWidth + colWidth / 2f + zigZagShiftX + shifterShiftX * colWidth, // todo simplify
                    0f, 
                    localRowIndex * colHeight);

                tileRow[i] = PlaceTile(position);
            }
        }
        else
        {
            for (int i = 0; i < colCount; i++)
            {
                float zigZagShiftZ = (((i + shifterShiftX) % 2 == 0) ? -1 : 1) * (colHeight / 4f);
                int localRowIndex = rowIndex % rowCount;

                var position = new Vector3(
                    -chunkWidth / 2f + i * colWidth + colWidth / 2f + shifterShiftX * colWidth, // todo simplify
                    0f,
                    localRowIndex * colHeight + zigZagShiftZ);

                tileRow[i] = PlaceTile(position);
            }
        }

        return tileRow;
    }

    GameObject PlaceTile(Vector3 position)
    {
        var instance = Instantiate(isPointTop ? pointTopPrefab : flatTopPrefab, transform);

        var scale = sideSize;
        instance.transform.localScale = new Vector3(scale, scale, scale);
        instance.transform.localPosition = position;

        return instance;
    }
}
