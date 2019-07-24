using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SquareTileChunkGenerator : ChunkGeneratorBase
{
    private List<GameObject[]> tileRowList = new List<GameObject[]>();

    private const int rowCount = 15;
    private int newestRowShift = 0;

    private Texture2D tex;

    public GameObject dirParticleSystem;

    protected override void CheckPlayersPositions()
    {
        base.CheckPlayersPositions();
        CheckTiles();
    }

    private void CheckTiles() // todo: optimize, check only when necessary
    {
        if (tileRowList.Count == 0)
        {
            return;
        }

        var lastRow = tileRowList[0];
        var lastRowZ = lastRow[1].transform.localPosition.z;

        var bestPlayerZ = 0.0f;
        foreach (var player in players)
        {
            if (player.currentChunkIndex != chunkIndex) // watch out for intersection with next chunk
            {
                continue;
            }

            var playerLocalPosition = transform.InverseTransformPoint(player.instance.transform.position);

            if (player.isAlive && playerLocalPosition.z > bestPlayerZ)
            {
                bestPlayerZ = playerLocalPosition.z;
            }
        }

        if (lastRowZ < bestPlayerZ - 22) //35
        {
            for (int i = 0; i < lastRow.Length; i++)
            {
                var tile = lastRow[i];
                if (tile == null) continue;
                var rb = tile.AddComponent<Rigidbody>(); // todo: pozdeji znicit uplne
                rb.mass = 1000;
                rb.AddTorque(Random.insideUnitSphere * 100000);
            }
            tileRowList.RemoveAt(0);
        }
    }

    protected override bool IsPlayerOnThisChunk(Vector3 localPosition)
    {
        if (localPosition.z >= 0 && localPosition.z <= localExitPosition.z
            && localPosition.x > -20 && localPosition.x < 20) // not super accurate
        {
            return true;
        }
        return false;
    }

    protected override bool IsPlayerAlive(Vector3 localPosition)
    {
        if (localPosition.y < -10)
        {
            return false;
        }
        return true;
    }

    private void Awake()
    {
        tex = Resources.Load("Textures/three_lines") as Texture2D;
    }

    public override void Init(int chunkIndex)
    {
        this.chunkIndex = chunkIndex;

        if (chunkIndex > 0)
        {
            rotationY = Random.Range(-30f, 30f);
            transform.Rotate(Vector3.up * rotationY);
        }

        for (int i = 0; i < rowCount; i++)
        {
            PlaceRowOfTiles(i);
        }
    }

    private void PlaceRowOfTiles(int row)
    {
        var tilesInRow = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            if ((chunkIndex > 0 || row > 3) && i != 1 && Random.Range(0, 15) == 0) continue;
            tilesInRow[i] = PlaceTile(new Vector3((i - 1) * 10 + newestRowShift * 5, 0f, row * 10), row % 4,
                row != rowCount - 1);
        }

        tileRowList.Add(tilesInRow);

        if (chunkIndex == 0 && row < 3)
        {
            return;
        }

        var randomObstacle = Random.Range(0, 6);
        if (randomObstacle < 2)
        {
            PlaceObstacleRandomly(row, randomObstacle);
        }
        if (randomObstacle > 2)
        {
            PlaceObstacleRandomly(row, Random.Range(0, 6));
        }

        if (row == rowCount - 1)
        {
            localExitPosition = new Vector3(newestRowShift * 5f, 0f, rowCount * 10f);
            worldExitPosition = transform.TransformPoint(localExitPosition);
            return;
        }

        // separate func?
        /*var sceneryObject = PillarSceneryGenerator.GenerateRow(row);
        sceneryObject.transform.parent = transform;
        sceneryObject.transform.eulerAngles = transform.eulerAngles;
        sceneryObject.transform.localPosition = new Vector3(/*newestRowShift * 5*-/ 0, 0, row * 10f);*/


        //newestRowShift += Random.Range(-1, 3) % 2;
        //newestRowShift += Random.Range(2, 4) % 3 - 1;
        newestRowShift += Random.Range(-3, 4) % 2;
    }

    private GameObject PlaceTile(Vector3 position, int type, bool withTrails)
    {
        var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = transform;
        instance.transform.eulerAngles = transform.eulerAngles;

        instance.transform.localScale = new Vector3(10, 1, 10);
        instance.transform.localPosition = position;

        instance.SetColor(Random.ColorHSV(type * 0.25f, type * 0.25f + 0.05f, 0.1f, 0.2f, 0.9f, 1, 1, 1));

        if ((chunkIndex > 0 || position.z > 50) && Random.Range(0, 25) == 0)
        {
            var axis = (Random.Range(0, 2) == 0) ? Vector3.forward : Vector3.right;
            instance.transform.Rotate(axis * Random.Range(-15f, 15f));
        }

        if (withTrails)
        {
            Instantiate(dirParticleSystem, instance.transform);
        }

        return instance;
    }

    private void PlaceObstacleRandomly(int row, int type)
    {
        var instance = GameObject.CreatePrimitive((type == 0) ? PrimitiveType.Sphere : PrimitiveType.Cube);
        instance.transform.parent = transform;
        instance.transform.eulerAngles = transform.eulerAngles;
        instance.transform.localPosition = new Vector3(Random.Range(-14f, 14f) + newestRowShift * 5, 2, row * 10 + Random.Range(-4f, 4f));

        var scale = Random.Range(2f, 5f);
        if (type < 2)
        {
            instance.transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            instance.transform.localScale = new Vector3(Random.Range(0.5f, 3f), Random.Range(0.5f, 3f), Random.Range(0.5f, 3f));
        }

        instance.transform.Rotate(Vector3.up * Random.Range(0f, 360f));
        //instance.SetColor(Random.ColorHSV(0, 1, 0, 0.1f, 0.9f, 1, 1, 1));
        instance.GetComponent<Renderer>().material.mainTexture = tex;
        instance.AddComponent(typeof(AnimateTiledTexture));

        var rb = instance.AddComponent<Rigidbody>();
        rb.mass = scale * 3;
    }
}
