using UnityEngine;
using System.Collections.Generic;

public class HexyPart : MonoBehaviour
{
    public GameManager gm; // just players maybe?
    public GameObject[] chunkPrefabs;

    public int rowInstantiateCount = 64; // todo by distance, not row count
    public int rowsInFrontOfBestPlayer = 48; // todo by distance, not row count ?

    List<HexyTileChunk> chunks = new List<HexyTileChunk>();
    List<GameObject[]> rows = new List<GameObject[]>(); // maybe array

    int latestVisitedRow = 0; // max row index visited by best player
    int nextRowIndex = 0;

    Vector3 nextRowPosition = new Vector3();

    IRowShifter rowShifter = new ZigZagRowShifter(5);
    int shifterShiftX = 0; // doesnt seem right

    ITileColorPatterner tileColorPatterner = new GrayWhiteTileColorPatterner();

    // IHoleMaker holeMaker = new RandomHoleMaker();

    private void Start()
    {
        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            var go = Instantiate(chunkPrefabs[i],
            nextRowPosition,
            Quaternion.identity) as GameObject;

            var hexChunk = go.GetComponent<HexyTileChunk>();

            chunks.Add(hexChunk);
        }
    }

    private void Update()
    {
        GetLatestVisitedRow();

        var newRowCount = rowsInFrontOfBestPlayer - (nextRowIndex - latestVisitedRow);
        if (newRowCount <= 0) return;

        for(int i = 0; i < newRowCount; i++)
        {
            // apply row shifter  ... maybe just do in HexyTileChunk ??
            shifterShiftX += rowShifter?.GetRowShiftX(nextRowIndex) ?? 0;

            // add
            var row = chunks[0].PlaceRow(nextRowPosition, nextRowIndex++, shifterShiftX);

            // apply color patterner ... maybe just do in HexyTileChunk also
            for (int j = 0; j < row.Length; j++)
            {
                var tile = row[j];
                if (tile == null) continue;

                tile.SetColor(tileColorPatterner.GetTileColor(nextRowIndex - 1, j, shifterShiftX));
                //tileColorPatterner
            }

            rows.Add(row);
            nextRowPosition.z += chunks[0].colHeight;

            // remove
            if (rows.Count <= rowInstantiateCount)
            {
                continue;
            }

            var lastRow = rows[0];

            for (int j = 0; j < lastRow.Length; j++)
            {
                var tile = lastRow[j];
                if (tile == null) continue;
                // animate and shit
                Object.Destroy(tile);
            }
            rows.RemoveAt(0);
        }
    }

    private void GetLatestVisitedRow()
    {
        foreach (var player in gm.players)
        {
            var playerRow = player.instance.GetComponent<HoverSailController>().rowIndex;
            if (playerRow > latestVisitedRow)
            {
                latestVisitedRow = playerRow;
            }
        }
    }
}
