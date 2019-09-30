using UnityEngine;
using System.Collections.Generic;

public class HexyPart : MonoBehaviour
{
    public GameManager gm; // just players maybe?
    public GameObject[] chunkPrefabs;

    public int rowInstantiateCount = 64; // todo by distance, not row count
    public int rowsInFrontOfBestPlayer = 48; // todo by distance, not row count ?

    List<HexGridRowGenerator> chunks = new List<HexGridRowGenerator>();
    List<TrackRow> rows = new List<TrackRow>(); // maybe array

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

            var hexChunk = go.GetComponent<HexGridRowGenerator>();

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
            PossiblyRandomize(); //

            // add
            var row = chunks[0].PlaceRow(nextRowIndex++);
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
        foreach (var player in gm.players)
        {
            var playerRow = player.instance.GetComponent<HoverSailController>().rowIndex;
            if (playerRow > latestVisitedRow)
            {
                latestVisitedRow = playerRow;
            }
        }
    }

    // todo: create some randomizer class...

    int nextRadomizeIndex = 0;

    private void PossiblyRandomize()
    {
        if (nextRowIndex == nextRadomizeIndex)
        {
            RandomizeGenerator(chunks[0], 30, 2, 12);
            chunks[0].nextRowPosition.y -= 3f;
            // chunks[0].nextRowRotation = Quaternion.Euler(0, Random.Range(-10f, 10f), 0);
        }

        var patterner = chunks[0].tileColorPatterner as TwoColorGridPatterner;
        if (patterner != null) patterner.colors = palette.Get2Colors(nextRowIndex);
    }

    IGridColorPatterner[] patterners = new IGridColorPatterner[]
    {
        // new RandomGridColorPatterner(),
        new ChessGridColorPatterner(),
        new ChessGridNoShiftColorPatterner(),
        new StripesColorPatterner(),
        new StripesNoShiftPatterner()
    };

    IRowShifter[] shifters = new IRowShifter[]
    {
        // new RandomRowShifter(),
        new ZigZagRowShifter(1),
        new ZigZagRowShifter(2),
        new ZigZagRowShifter(4),
        null
        // new ZigZagRowShifter(8),
    };

    ColorPalette palette = new ColorPalette();

    private void RandomizeGenerator(HexGridRowGenerator generator, float trackWidth, int minCols, int maxCols)
    {
        bool isPointTop = Random.Range(0, 2) == 0;
        int colCount = Random.Range(minCols, maxCols + 1); // constraint these

        var colWidth = trackWidth / colCount;

        var sideSize = isPointTop
            ? colWidth / Mathf.Sqrt(3f)
            : colWidth / 1.5f;

        generator.isPointTop = isPointTop;
        generator.isReversed = Random.Range(0, 2) == 0;
        generator.sideSize = sideSize;
        generator.colCount = colCount;
        generator.RecomputeValues();


        generator.rowShifter = shifters[Random.Range(0, shifters.Length)];
        generator.tileColorPatterner = patterners[Random.Range(0, patterners.Length)];


        //var patterner = generator.tileColorPatterner as TwoColorGridPatterner;
        //if (patterner != null) patterner.colors = palette.Get2Colors(nextRowIndex);


        nextRadomizeIndex += Random.Range(2 * colCount, 4 * colCount);
    }
}
