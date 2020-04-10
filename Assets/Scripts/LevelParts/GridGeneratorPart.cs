using UnityEngine;
using System.Collections.Generic;

public class GridGeneratorPart : MonoBehaviour
{
    public GameManager gm; // just players maybe?
    public RandomizerBase randomizer;

    public int rowInstantiateCount = 128; // todo by distance, not row count
    public int rowsInFrontOfBestPlayer = 96; // todo by distance, not row count ?

    List<TrackRow> rows = new List<TrackRow>(); // maybe array

    int latestVisitedRow = 0; // max row index visited by best player
    int nextRowIndex = 0; // next generated row index

    // Vector3 nextRowPosition = new Vector3();
    // Quaternion nextChunkRotation = Quaternion.identity;

    // GridRowGenerator currentGenerator;

    private void Update()
    {
        GetLatestVisitedRow();

        var newRowCount = rowsInFrontOfBestPlayer - (nextRowIndex - latestVisitedRow);
        if (newRowCount <= 0) return;

        for(int i = 0; i < newRowCount; i++)
        {
            // add
            var row = randomizer.GetRow(nextRowIndex++);
            rows.Add(row);

            // remove
            if (rows.Count <= rowInstantiateCount)
            {
                continue;
            }

            var lastRow = rows[0];
            Object.Destroy(lastRow.gameObject);
            rows.RemoveAt(0);
        }
    }

    private void GetLatestVisitedRow()
    {
        var rowCount = rows.Count;
        if(rowCount == 0)
        {
            return;
        }

        foreach (var player in gm.players)
        {

            if(!player.isAlive)
            {
                continue;
            }

            // consider other ways
            if (player.instance.transform.position.y < rows[rowCount - 1].transform.position.y - 2f)
            {
                player.Kill();
            }

            var playerRow = player.instance.GetComponent<HoverSailController>().rowIndex;
            if (playerRow > latestVisitedRow)
            {
                latestVisitedRow = playerRow;
            }
        }
    }
}
