using UnityEngine;

public class HexyTileChunk : MonoBehaviour // HexTilePathGenerator
{
    public GameObject pointTopPrefab;
    public GameObject flatTopPrefab;

    public bool isPointTop;
    // public bool endsWithCurve; // modifier
    // public bool endCurveGoesLeft; // also?

    // public int rowCount;
    public int colCount;
    public float sideSize;

    float chunkWidth;

    float colWidth;
    float colHeight;

    public Vector3 nextRowPosition = Vector3.zero;
    public Quaternion nextRowRotation = Quaternion.identity;

    IRowShifter rowShifter = new ZigZagRowShifter(4);
    int shifterShiftX = 0;

    ITileColorPatterner tileColorPatterner = new GrayWhiteTileColorPatterner();

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

    public GameObject[] PlaceRow(int rowIndex, int colsFromLeft = 0)
    {
        var tileRow = new GameObject[(colsFromLeft == 0) ? colCount : Mathf.Abs(colsFromLeft)];

        var newShifterShiftX = (colsFromLeft != 0) 
            ? 0 
            : (rowShifter?.GetRowShiftX(rowIndex) ?? 0);
        shifterShiftX += newShifterShiftX;

        var forStart = (colsFromLeft >= 0) ? 0 : colCount - Mathf.Abs(colsFromLeft);
        var forEnd = (colsFromLeft <= 0) ? colCount : Mathf.Abs(colsFromLeft);
        var forIterations = 0;

        for (int i = forStart; i < forEnd; i++)
        {
            Vector3 tilePosition = new Vector3(
                -chunkWidth / 2f + i * colWidth/* + colWidth / 2f*/ + newShifterShiftX * colWidth,
                0f,
                0f);

            if (isPointTop)
            {
                var zigZagShiftX = ((rowIndex % 2 == 0) ? 0 : 1) * (colWidth / 2f);
                tilePosition.x += zigZagShiftX;
            }
            else
            {
                var zigZagShiftZ = (((i + shifterShiftX) % 2 == 0) ? 0 : 1) * (colHeight / 2f);
                tilePosition.z += zigZagShiftZ;
            }
                
            var tile = PlaceTile(tilePosition);

            tile.SetColor(tileColorPatterner.GetTileColor(rowIndex, i, shifterShiftX));
            tile.GetComponent<HexyTile>().rowIndex = rowIndex; // ???

            tileRow[forIterations++] = tile;
        }

        var addPosition = nextRowRotation * new Vector3(newShifterShiftX * colWidth, 0f, colHeight);
        nextRowPosition += addPosition;

        return tileRow;
    }

    // WIP
    public void PlaceCurveRows(int rowIndex, bool isCurveLeft) // todo return rows
    {
        for (int i = 1; i < colCount / 2; i++)
        {
            PlaceRow(rowIndex++, isCurveLeft ? -(colCount - i * 2) : (colCount - i * 2)); // apply zigZagShiftX...
        }
    }

    /*if (nextRowIndex == 30)
    {
        nextRowRotation = Quaternion.Euler(0, 30, 0);
        chunks[0].isPointTop = !chunks[0].isPointTop;

        var swap = chunks[0].colWidth;
        chunks[0].colWidth = chunks[0].colHeight;
        chunks[0].colHeight = swap;
        chunks[0].chunkWidth = chunks[0].colWidth * chunks[0].colCount;
    }*/

    GameObject PlaceTile(Vector3 position)
    {
        var instance = Instantiate(isPointTop ? pointTopPrefab : flatTopPrefab, transform);

        var scale = sideSize;
        instance.transform.localScale = new Vector3(scale, scale, scale);
        instance.transform.localPosition = nextRowRotation * position + nextRowPosition;
        instance.transform.localRotation = nextRowRotation;

        return instance;
    }
}
