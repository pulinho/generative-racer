using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossRandomizer : RandomizerBase
{
    // not sure...
    public GameManager gm;

    public GameObject[] generatorPrefabs;
    List<GridRowGenerator> generators = new List<GridRowGenerator>();

    //public ObstacleMaker obstacler;

    public float trackWidth = 30f;
    public int minTilesPerRow = 3;
    public int maxTilesPerRow = 12;

    //public Texture2D animTex;

    IGridPatterner[] patterners = new IGridPatterner[]
    {
        //new RandomGridColorPatterner(),
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
        null,
    };

    Color3Palette palette3 = new Color3Palette();
    Color2Palette palette2 = new Color2Palette();

    IScenery[] sceneries = new IScenery[]
    {
        new NuerScenery()
    };
    IScenery currentScenery;

    int nextRadomizeIndex = 0;

    GridRowGenerator currentGenerator;

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

    public override TrackRow GetRow(int rowIndex)
    {
        // todo less shitty
        if (rowIndex != 0 && rowIndex == nextRadomizeIndex)
        {
            UpdateGeneratorForRow(rowIndex); ///
            //return PlaceInterChunkObstacle(rowIndex);
        }

        UpdateGeneratorForRow(rowIndex);

        var row = currentGenerator.PlaceRow(rowIndex);

        // maybe put to generator?
        /*if (rowIndex % Mathf.CeilToInt(5f / currentGenerator.sideSize) == 0)
        {
            var sceneryObject = PillarTileChunkScenery.GenerateRow(fakeRowIndex++);
            sceneryObject.transform.parent = row.transform;
            sceneryObject.transform.position = row.transform.position;
            // sceneryObject.transform.eulerAngles = row.transform.eulerAngles;

            // sceneryObject.SetColor(palette3.Get3Colors(rowIndex)[1]);
        }*/

        var sceneryObject = sceneries[Random.Range(0, sceneries.Length)].GenerateRow(rowIndex);
        sceneryObject.transform.parent = row.transform;
        sceneryObject.transform.position = row.transform.position;
        //sceneryObject.SetColor(palette3.Get3Colors(rowIndex)[2]);

        if (rowIndex % 100 == 99)
        {
            BonusMaker.PlaceBonus(row.gameObject, rowIndex, gm);
        }
        else
        {
            //obstacler?.PlaceObstacle(row.gameObject, rowIndex);
        }

        return row;
    }

    public void UpdateGeneratorForRow(int rowIndex)
    {
        if (rowIndex >= nextRadomizeIndex)
        {
            var nextRowPosition = currentGenerator?.nextRowPosition ?? Vector3.zero;
            nextRowPosition.y -= 3f;
            // nextRowPosition.z -= currentGenerator?.sideSize ?? 0f;

            currentGenerator = generators[Random.Range(0, generators.Count)];
            currentGenerator.nextRowPosition = nextRowPosition;

            //currentScenery = sceneries[Random.Range(0, sceneries.Length)];

            RandomizeGenerator(currentGenerator, trackWidth, minTilesPerRow, maxTilesPerRow);
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

    /*private TrackRow PlaceInterChunkObstacle(int rowIndex)
    {
        var go = new GameObject();
        go.transform.localPosition = currentGenerator.nextRowPosition; // todo: shift next row
        // go.transform.localRotation = nextRowRotation;

        var frr = go.AddComponent<FanRowReplacement>();

        frr.animTex = animTex;


        var tr = go.AddComponent<TrackRow>();
        tr.rowIndex = rowIndex;

        currentGenerator.SkipRow();

        return tr;
    }*/
}
