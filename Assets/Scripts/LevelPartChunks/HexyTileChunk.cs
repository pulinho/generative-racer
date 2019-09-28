using UnityEngine;

public class HexyTileChunk : MonoBehaviour // HexTilePathGenerator
{
    public GameObject pointTopPrefab;
    public GameObject flatTopPrefab;

    public bool isPointTop;
    public bool endsWithCurve; // modifier
    public bool endCurveGoesLeft; // also?

    // public int rowCount;
    public int colCount;

    public float sideSize;

    [HideInInspector]
    public float chunkWidth;

    [HideInInspector]
    public float colWidth; // make private set
    [HideInInspector]
    public float colHeight;

    public Vector3 nextRowPosition = Vector3.zero;
    public Quaternion nextRowRotation = Quaternion.identity;

    IRowShifter rowShifter = new ZigZagRowShifter(8);
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

    public GameObject[] PlaceRow(int rowIndex)
    {
        var tileRow = new GameObject[colCount];

        var newShifterShiftX = rowShifter?.GetRowShiftX(rowIndex) ?? 0;
        shifterShiftX += newShifterShiftX;

        if (isPointTop) // separate classes?
        {
            float zigZagShiftX = ((rowIndex % 2 == 0) ?  0 : 1) * (colWidth / 2f);

            for (int i = 0; i < colCount; i++)
            {
                var position = new Vector3(
                    -chunkWidth / 2f + i * colWidth/* + colWidth / 2f*/ + zigZagShiftX + newShifterShiftX * colWidth, // todo simplify
                    0f,
                    0f);

                tileRow[i] = PlaceTile(position);

                tileRow[i].SetColor(tileColorPatterner.GetTileColor(rowIndex, i, shifterShiftX));
                tileRow[i].GetComponent<HexyTile>().rowIndex = rowIndex; // ???
            }
        }
        else
        {
            for (int i = 0; i < colCount; i++)
            {
                float zigZagShiftZ = (((i + shifterShiftX) % 2 == 0) ? 0 : 1) * (colHeight / 2f);

                var position = new Vector3(
                    -chunkWidth / 2f + i * colWidth/* + colWidth / 2f*/ + newShifterShiftX * colWidth, // todo simplify
                    0f,
                    0f + zigZagShiftZ);

                tileRow[i] = PlaceTile(position);

                tileRow[i].SetColor(tileColorPatterner.GetTileColor(rowIndex, i, shifterShiftX));
                tileRow[i].GetComponent<HexyTile>().rowIndex = rowIndex; // ???
            }
        }

        var addPosition = nextRowRotation * new Vector3(newShifterShiftX * colWidth, 0f, colHeight); // + shifter shit
        nextRowPosition += addPosition;

        return tileRow;
    }

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
