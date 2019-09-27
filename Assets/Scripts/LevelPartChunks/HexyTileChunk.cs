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

    float chunkWidth;

    [HideInInspector]
    public float colWidth; // make private se
    [HideInInspector]
    public float colHeight;

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

    public GameObject[] PlaceRow(Vector3 placePosition, int rowIndex, int shifterShiftX = 0)
    {
        var tileRow = new GameObject[colCount];

        if (isPointTop) // separate classes?
        {
            float zigZagShiftX = ((rowIndex % 2 == 0) ?  -1 : 1) * (colWidth / 4f);

            for (int i = 0; i < colCount; i++)
            {
                var position = new Vector3(
                    -chunkWidth / 2f + i * colWidth + colWidth / 2f + zigZagShiftX + shifterShiftX * colWidth, // todo simplify
                    0f,
                    0f);

                tileRow[i] = PlaceTile(position + placePosition);

                tileRow[i].GetComponent<HexyTile>().rowIndex = rowIndex; // ???
            }
        }
        else
        {
            for (int i = 0; i < colCount; i++)
            {
                float zigZagShiftZ = (((i + shifterShiftX) % 2 == 0) ? 1 : -1) * (colHeight / 4f);

                var position = new Vector3(
                    -chunkWidth / 2f + i * colWidth + colWidth / 2f + shifterShiftX * colWidth, // todo simplify
                    0f,
                    0f + zigZagShiftZ);

                tileRow[i] = PlaceTile(position + placePosition);

                tileRow[i].GetComponent<HexyTile>().rowIndex = rowIndex; // ???
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
