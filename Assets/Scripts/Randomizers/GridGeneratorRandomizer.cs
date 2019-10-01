using UnityEngine;
using System.Collections.Generic;

public class GridGeneratorRandomizer : MonoBehaviour
{
    public GameObject[] generatorPrefabs;
    List<GridRowGenerator> generators = new List<GridRowGenerator>();

    IGridPatterner[] patterners = new IGridPatterner[]
    {
        // new RandomGridColorPatterner(),
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

    int nextRadomizeIndex = 0;

    GridRowGenerator currentGenerator;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < generatorPrefabs.Length; i++)
        {
            var go = Instantiate(generatorPrefabs[i]) as GameObject;

            var generator = go.GetComponent<GridRowGenerator>();

            generators.Add(generator);
        }
    }

    int fakeRowIndex = 0; // fujky

    public TrackRow GetRow(int rowIndex)
    {
        UpdateGeneratorForRow(rowIndex);

        var row = currentGenerator.PlaceRow(rowIndex);

        if (rowIndex % Mathf.CeilToInt(5f / currentGenerator.sideSize) == 0)
        {
            var sceneryObject = PillarTileChunkScenery.GenerateRow(fakeRowIndex++);
            sceneryObject.transform.parent = row.transform;
            sceneryObject.transform.position = row.transform.position;
            // sceneryObject.transform.eulerAngles = row.transform.eulerAngles;

            // sceneryObject.SetColor(palette3.Get3Colors(rowIndex)[1]);
        }

        return row;
    }

    public void UpdateGeneratorForRow(int rowIndex)
    {
        if (rowIndex == nextRadomizeIndex)
        {
            var nextRowPosition = currentGenerator?.nextRowPosition ?? Vector3.zero;
            nextRowPosition.y -= 2.5f;
            nextRowPosition.z -= currentGenerator?.sideSize ?? 0f;

            currentGenerator = generators[Random.Range(0, generators.Count)];
            currentGenerator.nextRowPosition = nextRowPosition;

            RandomizeGenerator(currentGenerator, 30, 3, 12);
        }

        var patterner = currentGenerator.patterner as ThreeColorGridPatterner;
        if (patterner != null) patterner.colors = palette3.Get3Colors(rowIndex);

        ///
        var patterner2 = currentGenerator.patterner as TwoColorGridPatterner;
        if (patterner2 != null) patterner2.colors = palette2.Get2Colors(rowIndex);
    }

    void RandomizeGenerator(GridRowGenerator generator, float trackWidth, int minCols, int maxCols)
    {
        bool isPointTop = Random.Range(0, 2) == 0;
        int colCount = Random.Range(minCols, maxCols + 1); // constraint these

        var colWidth = trackWidth / colCount;

        float sideSize = 1f;
        if (generator is SquareGridRowGenerator)
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


        //var patterner = generator.patterner as TwoColorGridPatterner;
        //if (patterner != null) patterner.colors = palette2.Get2Colors(Random.Range(0, 77));

        var mult = (generator is SquareGridRowGenerator && isPointTop ? 1 : 2);
        nextRadomizeIndex += Random.Range(colCount * mult, colCount * mult * 3);
    }
}
