using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexTileChunkGenerator : ChunkGeneratorBase
{
    public GameObject[] tilePrefabs;

    private List<GameObject[]> tileRowList;

    private const int rowCount = 20;

    private Texture2D texSphere;
    private Texture2D texCube;

    protected override void CheckPlayersPositions()
    {
        base.CheckPlayersPositions();
        CheckTiles();
    }

    private void CheckTiles() // todo: optimize, check only when necessary
    {
        if(tileRowList.Count == 0)
        {
            return;
        }

        var lastRow = tileRowList[0];
        var lastRowZ = lastRow[1].transform.localPosition.z;

        var bestPlayerZ = 0.0f;
        foreach (var player in players)
        {
            var playerLocalPosition = transform.InverseTransformPoint(player.instance.transform.position);

            if (player.isAlive && playerLocalPosition.z > bestPlayerZ)
            {
                bestPlayerZ = playerLocalPosition.z;
            }
        }

        if (lastRowZ < bestPlayerZ - 10) //35
        {
            for (int i = 0; i < lastRow.Length; i++)
            {
                var tile = lastRow[i];
                if (tile == null) continue;
                var rb = lastRow[i].AddComponent<Rigidbody>(); // todo: pozdeji znicit uplne
                rb.mass = 1000;
                rb.AddTorque(Random.insideUnitSphere * 100000);
            }
            tileRowList.RemoveAt(0);
        }
    }

    protected override bool IsPlayerAlive(Vector3 localPosition)
    {
        if(localPosition.z < 0 || localPosition.z > localExitPosition.z) // perhaps a separate (base?) method for detecting if player is even in chunk?
        { // perhaps remove the < 0
            return true;
        }
        if(localPosition.y < -5)
        {
            return false;
        }
        return true;
    }

    private void Awake()
    {
        texSphere = Resources.Load("Textures/cm2") as Texture2D;
        texCube = Resources.Load("Textures/tilted_squares") as Texture2D;

        transform.Rotate(Vector3.up * Random.Range(-10f, 10f));

        tileRowList = new List<GameObject[]>();

        PlaceInitialTiles(rowCount);
    }

    private void PlaceInitialTiles(int rowCount) 
    {
        int rowShift = 0;

        for (int i = 0; i < rowCount; i++)
        {
            StartCoroutine(PlaceRowOfTiles(i, rowShift));
            if (i < 5)
            {
                rowShift += (i % 2) * 2 - 1;
                continue;
            }
            if (i < rowCount - 1)
            {
                rowShift += Random.Range(2, 4) % 3 - 1;
            }
        }

        localExitPosition = new Vector3(rowShift * 4.33f, 0f, rowCount * 7.5f);
        worldExitPosition = transform.TransformPoint(localExitPosition);
    }

    private IEnumerator PlaceRowOfTiles(int row, int rowShift)
    {
        yield return new WaitForSeconds(0.15f * row);

        var tilesInRow = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            if (row > 5 && i != 1 && Random.Range(0, 12) == 0) continue;

            var position = new Vector3((i - 1) * 8.66f + rowShift * 4.33f, 0f, row * 7.5f);
            tilesInRow[i] = PlaceTile(position, (row / 10) % 3);
        }

        tileRowList.Add(tilesInRow);

        if (row > 5)
        {
            var randomObstacle = Random.Range(0, 20);
            if (randomObstacle < 2)
            {
                PlaceObstacleRandomly(row, randomObstacle, rowShift);
            }
            if (randomObstacle > 16)
            {
                PlaceObstacleRandomly(row, Random.Range(0, 6), rowShift);
            }
        }
    }

    private GameObject PlaceTile(Vector3 position, int type)
    {
        var tile = tilePrefabs[type % tilePrefabs.Length];

        var instance = Instantiate(tile, transform);

        instance.transform.localScale = new Vector3(5f, 1, 5f);
        instance.transform.localPosition = position;

        //instance.SetColor(Random.ColorHSV(type* 0.25f, type*0.25f + 0.05f, 0.1f, 0.2f, 0.9f, 1, 1, 1));

        if (position.z > 50 && Random.Range(0, 30) == 0)
        {
            var axis = (Random.Range(0, 2) == 0) ? Vector3.forward : Vector3.right;
            instance.transform.Rotate(axis * Random.Range(-15f, 15f));
        }

        return instance;
    }

    private void PlaceObstacleRandomly(int row, int type, int rowShift)
    {
        var instance = GameObject.CreatePrimitive((type == 0) ? PrimitiveType.Sphere : PrimitiveType.Cube);
        instance.transform.parent = transform; //
        instance.transform.eulerAngles = transform.eulerAngles; //
        instance.transform.localPosition = new Vector3(Random.Range(-8f, 8f) + rowShift * 4.33f, 2, row * 7.5f + Random.Range(-2f, 2f));

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

        if (type == 0)
        {
            instance.GetComponent<Renderer>().material.mainTexture = texSphere;
            instance.AddComponent(typeof(AnimateTiledTexture));
        }
        else
        {
            instance.GetComponent<Renderer>().material.mainTexture = texCube;
            var anim = instance.AddComponent(typeof(AnimateTiledTexture)) as AnimateTiledTexture;
            anim.rows = 4;
            anim.columns = 4;
            anim.frameCount = 16;
        }

        var rb = instance.AddComponent<Rigidbody>();
        rb.mass = scale * 3;
    }
}
