using UnityEngine;
using System.Collections.Generic;

public class GridGeneratorPart : MonoBehaviour
{
    public GameManager gm; // just players maybe?
    public GameObject[] generatorPrefabs;

    public int rowInstantiateCount = 64; // todo by distance, not row count
    public int rowsInFrontOfBestPlayer = 48; // todo by distance, not row count ?

    List<GridRowGenerator> generators = new List<GridRowGenerator>();
    List<TrackRow> rows = new List<TrackRow>(); // maybe array

    int latestVisitedRow = 0; // max row index visited by best player
    int nextRowIndex = 0; // next generated row index

    Vector3 nextRowPosition = new Vector3();
    Quaternion nextChunkRotation = Quaternion.identity;

    GridRowGenerator currentGenerator;

    private void Start()
    {
        for (int i = 0; i < generatorPrefabs.Length; i++)
        {
            var go = Instantiate(generatorPrefabs[i],
            nextRowPosition,
            nextChunkRotation) as GameObject;

            var generator = go.GetComponent<GridRowGenerator>();

            generators.Add(generator);
        }
    }

    private void Update()
    {
        GetLatestVisitedRow();

        var newRowCount = rowsInFrontOfBestPlayer - (nextRowIndex - latestVisitedRow);
        if (newRowCount <= 0) return;

        for(int i = 0; i < newRowCount; i++)
        {
            //PossiblyRandomize(); //

            if (nextRowIndex == nextRadomizeIndex)
            {
                nextRowPosition = currentGenerator?.nextRowPosition ?? Vector3.zero;
                nextRowPosition.y -= 1.5f;

                currentGenerator = GetRandomizedGenerator();
                currentGenerator.nextRowPosition = nextRowPosition;

                var patterner = currentGenerator.patterner as ThreeColorGridPatterner;
                if (patterner != null) patterner.colors = palette3.Get3Colors(nextRowIndex);
            }

            // add
            var row = currentGenerator.PlaceRow(nextRowIndex++);
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

    private GridRowGenerator GetRandomizedGenerator()
    {
        var generator = generators[Random.Range(0, generators.Count)];

        RandomizeGenerator(generator, 30, 3, 12);

        return generator;
    }

    IGridPatterner[] patterners = new IGridPatterner[]
    {
        new RandomGridColorPatterner(),
        new ChessPatterner(),
        new ChessNoShiftPatterner(),
        new StripesPatterner(),
        new StripesNoShiftPatterner(),

        new Grid3Patterner()
    };

    IRowShifter[] shifters = new IRowShifter[]
    {
        new RandomRowShifter(),
        new ZigZagRowShifter(1),
        new ZigZagRowShifter(2),
        new ZigZagRowShifter(4),
        new ZigZagRowShifter(6),
        // null,
    };

    Color3Palette palette3 = new Color3Palette();
    Color2Palette palette2 = new Color2Palette();

    private void RandomizeGenerator(GridRowGenerator generator, float trackWidth, int minCols, int maxCols)
    {
        bool isPointTop = Random.Range(0, 2) == 0;
        int colCount = Random.Range(minCols, maxCols + 1); // constraint these

        var colWidth = trackWidth / colCount;

        float sideSize = 1f;
        if(generator is SquareGridRowGenerator)
        {
            sideSize = isPointTop
                ? colWidth / Mathf.Sqrt(2f) * 2f
                : colWidth;
        }
        else if (generator is HexGridRowGenerator)
        {
            sideSize = isPointTop
                ? colWidth / Mathf.Sqrt(3f)
                : colWidth / 1.5f;
        }

        generator.isPointTop = isPointTop;
        generator.isReversed = Random.Range(0, 2) == 0;
        generator.sideSize = sideSize;
        generator.colCount = colCount;
        generator.RecomputeValues();


        generator.rowShifter = shifters[Random.Range(0, shifters.Length)];
        generator.patterner = patterners[Random.Range(0, patterners.Length)];


        var patterner = generator.patterner as TwoColorGridPatterner;
        if (patterner != null) patterner.colors = palette2.Get2Colors(Random.Range(0, 77));

        var mult = (generator is SquareGridRowGenerator && isPointTop ? 1 : 2);
        nextRadomizeIndex += Random.Range(colCount * mult, colCount * mult * 3);
    }
}
