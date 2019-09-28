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

    Vector3 nextChunkPosition = new Vector3();
    Quaternion nextChunkRotation = Quaternion.identity;

    private void Start()
    {
        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            var go = Instantiate(chunkPrefabs[i],
            nextChunkPosition,
            nextChunkRotation) as GameObject;

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
            // add
            var row = chunks[0].PlaceRow(nextRowIndex++);
            rows.Add(row);


            /*if (nextRowIndex == 30)
            {
                nextRowRotation = Quaternion.Euler(0, 30, 0);
                chunks[0].isPointTop = !chunks[0].isPointTop;

                var swap = chunks[0].colWidth;
                chunks[0].colWidth = chunks[0].colHeight;
                chunks[0].colHeight = swap;
                chunks[0].chunkWidth = chunks[0].colWidth * chunks[0].colCount;
            }*/


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
