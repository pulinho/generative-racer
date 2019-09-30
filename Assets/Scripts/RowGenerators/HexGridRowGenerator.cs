using UnityEngine;

public class HexGridRowGenerator : MonoBehaviour
{
    public GameObject pointTopPrefab;
    public GameObject flatTopPrefab;

    public bool isPointTop;
    public bool isReversed;

    public int colCount;
    public float sideSize;

    float rowWidth;

    float colWidth;
    float colHeight;

    public Vector3 nextRowPosition = Vector3.zero;
    public Quaternion nextRowRotation = Quaternion.identity;

    public IRowShifter rowShifter; // = new ZigZagRowShifter(4);
    int shifterShiftX = 0;

    public IGridColorPatterner tileColorPatterner; // = new GrayWhiteGridColorPatterner();

    void Awake()
    {
        RecomputeValues();
    }

    public void RecomputeValues()
    {
        colWidth = isPointTop
            ? sideSize * Mathf.Sqrt(3f)
            : sideSize * 1.5f; // * 2f;
        colHeight = isPointTop
            ? sideSize * 1.5f
            : sideSize * Mathf.Sqrt(3f); // / 2f;
        rowWidth = colWidth * colCount;
    }

    public TrackRow PlaceRow(int rowIndex)
    {
        var tileRow = new GameObject();
        tileRow.transform.localPosition = nextRowPosition;
        tileRow.transform.localRotation = nextRowRotation;

        var tr = tileRow.AddComponent<TrackRow>();
        tr.rowIndex = rowIndex;
        tr.rowLength = colHeight;

        var newShifterShiftX = rowIndex < 5 
            ? 0
            : rowShifter?.GetRowShiftX(rowIndex) ?? 0;

        shifterShiftX += newShifterShiftX;

        for (int i = 0; i < colCount; i++)
        {
            Vector3 tilePosition = new Vector3(
                -rowWidth / 2f + i * colWidth + colWidth / 2f + newShifterShiftX * colWidth,
                0f,
                0f);

            if (isPointTop)
            {
                var zigZagShiftX = ((rowIndex % 2 == 0) ? -1 : 1) * (colWidth / 4f) * (isReversed ? -1 : 1);
                tilePosition.x += zigZagShiftX;
            }
            else
            {
                var zigZagShiftZ = (((i + shifterShiftX) % 2 == 0) ? -1 : 1) * (colHeight / 4f) * (isReversed ? -1 : 1);
                tilePosition.z += zigZagShiftZ;
            }
                
            var tile = PlaceTile(tileRow.transform, tilePosition);

            tile.SetColor(tileColorPatterner.GetTileColor(rowIndex, i, shifterShiftX));
        }

        var addPosition = nextRowRotation * new Vector3(newShifterShiftX * colWidth, 0f, colHeight);
        nextRowPosition += addPosition;

        return tr;
    }

    GameObject PlaceTile(Transform parent, Vector3 position)
    {
        var instance = Instantiate(isPointTop ? pointTopPrefab : flatTopPrefab, parent);

        instance.transform.localScale = new Vector3(sideSize, sideSize, sideSize);
        instance.transform.localPosition = position;

        return instance;
    }
}
