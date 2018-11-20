using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLevelGenerator : MonoBehaviour {

    public GameManager gm;
    public Transform ground;
    
    private List<GameObject[]> tileRowList;

    private int newestRow = 15;
    private int newestRowShift = 0;

    private Texture2D tex;

    private void FixedUpdate()
    {
        var lastRow = tileRowList[0];
        var lastRowZ = lastRow[0].transform.position.z;

        var bestPlayerZ = 0.0f;
        foreach(var player in gm.players)
        {
            if(player.isAlive && player.instance.transform.position.z > bestPlayerZ)
            {
                bestPlayerZ = player.instance.transform.position.z;
            }
        }
        
        if(lastRowZ < bestPlayerZ - 35)
        {
            for (int i = 0; i < lastRow.Length; i++)
            {
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
        tex = Resources.Load("Textures/three_lines") as Texture2D;

        ground.transform.Rotate(Vector3.up * Random.Range(-10f, 10f));

        tileRowList = new List<GameObject[]>();

        for (int i = 0; i < newestRow; i++)
        {
            PlaceRowOfTiles(i);
        }
    }

    private void PlaceRowOfTiles(int row)
    {
        var tilesInRow = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            tilesInRow[i] = PlaceTile(new Vector3(i*10-10 + newestRowShift*5, 0f, row*10), row % 4);
        }

        tileRowList.Add(tilesInRow);

        if(row < 3)
        {
            return;
        }
        newestRowShift += Random.Range(-1, 3) % 2;

        var randomObstacle = Random.Range(0, 6);
        if (randomObstacle < 2)
        {
            PlaceObstacleRandomly(row, randomObstacle);
        }
        if (randomObstacle > 2)
        {
            PlaceObstacleRandomly(row, Random.Range(0, 6));
        }
    }

    private GameObject PlaceTile(Vector3 position, int type)
    {
        var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = ground.transform;
        instance.transform.eulerAngles = ground.transform.eulerAngles;

        instance.transform.localScale = new Vector3(10, 1, 10);
        instance.transform.localPosition = position;

        instance.SetColor(Random.ColorHSV(type* 0.25f, type*0.25f + 0.05f, 0.1f, 0.2f, 0.9f, 1, 1, 1));

        if(position.z > 50 && Random.Range(0, 30) == 0)
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
        instance.transform.localPosition = new Vector3(Random.Range(-14f, 14f) + newestRowShift * 5, 2, row*10 + Random.Range(-4f, 4f));

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
