using UnityEngine;

public class SquareGridRowGenerator : GridRowGenerator
{
    public GameObject flatTopPrefab;

    public override void RecomputeValues()
    {
        colWidth = isPointTop
            ? sideSize * Mathf.Sqrt(2f) / 2f // Mathf.Sqrt(2f)
            : sideSize;
        colHeight = isPointTop
            ? sideSize * Mathf.Sqrt(2f) // Mathf.Sqrt(2f) / 2f
            : sideSize;
        rowWidth = colWidth * colCount;
    }

    public override TrackRow PlaceRow(int rowIndex)
    {
        var tileRow = new GameObject();
        tileRow.transform.localPosition = nextRowPosition;
        tileRow.transform.localRotation = nextRowRotation;

        var tr = tileRow.AddComponent<TrackRow>();
        tr.rowIndex = rowIndex;
        tr.rowLength = colHeight;

        var newShifterShiftX = rowIndex < 5 // 5?
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
                var zigZagShiftZ = (((i + shifterShiftX) % 2 == 0) ? -1 : 1) * (colHeight / 4f) * (isReversed ? -1 : 1);
                tilePosition.z += zigZagShiftZ;
            }

            var tile = PlaceTile(tileRow.transform, tilePosition);

            tile.SetColor(patterner.GetTileColor(rowIndex, i, shifterShiftX));
        }

        var addPosition = nextRowRotation * new Vector3(newShifterShiftX * colWidth, 0f, colHeight);
        nextRowPosition += addPosition;

        return tr;
    }

    GameObject PlaceTile(Transform parent, Vector3 position)
    {
        var instance = Instantiate(flatTopPrefab, parent);

        instance.transform.localScale = new Vector3(sideSize, sideSize * 0.25f, sideSize); // !!
        instance.transform.localPosition = position;
        instance.transform.localRotation = Quaternion.Euler(0f, isPointTop ? 45f : 0f, 0f);

        return instance;
    }
}
