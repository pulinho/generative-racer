using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCarLevelGenerator : MonoBehaviour
{
    public GameManager gm;
    public Transform ground;
    public GameObject[] tilePrefabs;

    private List<GameObject[]> tileRowList;

    private int newestRow = 20;
    private int newestRowShift = 1;

    private Texture2D texSphere;
    private Texture2D texCube;

    private void FixedUpdate()
    {
        var lastRow = tileRowList[0];
        var lastRowZ = lastRow[1].transform.position.z;

        var bestPlayerZ = 0.0f;
        foreach (var player in gm.players)
        {
            if (player.isAlive && player.instance.transform.position.z > bestPlayerZ)
            {
                bestPlayerZ = player.instance.transform.position.z;
            }
        }

        if (lastRowZ < bestPlayerZ - 35)
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

            PlaceRowOfTiles(newestRow);
            newestRow++;
        }
    }

    private void Awake()
    {
        texSphere = Resources.Load("Textures/cm2") as Texture2D;
        texCube = Resources.Load("Textures/tilted_squares") as Texture2D;

        ground.transform.Rotate(Vector3.up * Random.Range(-10f, 10f));

        tileRowList = new List<GameObject[]>();

        /*for (int i = 0; i < newestRow; i++)
        {
            PlaceRowOfTiles(i);
        }*/
        StartCoroutine(PlaceInitialTiles(newestRow));
    }

    private IEnumerator PlaceInitialTiles(int rowCount)
    {
        for (int i = 0; i < rowCount; i++)
        {
            PlaceRowOfTiles(i);
            yield return new WaitForSeconds(0.06f);
        }
    }

    private void PlaceRowOfTiles(int row)
    {
        var tilesInRow = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            if (row > 5 && i != 1 && Random.Range(0, 12) == 0) continue;
            tilesInRow[i] = PlaceTile(new Vector3(i * 8.66f - 10f + newestRowShift * 4.33f, 0f, row * 7.5f), (row / 20) % 3);
        }

        tileRowList.Add(tilesInRow);

        if (row < 5)
        {
            newestRowShift += (row % 2) * 2 - 1;
            return;
        }
        newestRowShift += Random.Range(2, 4) % 3 - 1;

        var randomObstacle = Random.Range(0, 20);
        if (randomObstacle < 2)
        {
            PlaceObstacleRandomly(row, randomObstacle);
        }
        if (randomObstacle > 16)
        {
            PlaceObstacleRandomly(row, Random.Range(0, 6));
        }
    }

    private GameObject PlaceTile(Vector3 position, int type)
    {
        var tile = tilePrefabs[type % tilePrefabs.Length];

        var instance = Instantiate(tile, new Vector3(), Quaternion.identity);//GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = ground.transform;
        instance.transform.eulerAngles = ground.transform.eulerAngles;

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

    private void PlaceObstacleRandomly(int row, int type)
    {
        var instance = GameObject.CreatePrimitive((type == 0) ? PrimitiveType.Sphere : PrimitiveType.Cube);
        instance.transform.parent = ground.transform;
        instance.transform.eulerAngles = ground.transform.eulerAngles;
        instance.transform.localPosition = new Vector3(Random.Range(-8f, 8f) + newestRowShift * 4.33f, 2, row * 7.5f + Random.Range(-2f, 2f));

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
