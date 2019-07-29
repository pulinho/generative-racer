using UnityEngine;

public class HexTileChunk : TileChunkBase
{
    public GameObject[] tilePrefabs;

    public Texture2D texSphere;
    public Texture2D texCube;

    public override void Init(int chunkIndex)
    {
        this.chunkIndex = chunkIndex;

        if (chunkIndex > 0)
        {
            rotationY = Random.Range(-30f, 30f);
            transform.Rotate(Vector3.up * rotationY);
        }

        PlaceInitialTiles(rowCount);
    }

    private void PlaceInitialTiles(int rowCount) 
    {
        int rowShift = 0;

        for (int i = 0; i < rowCount; i++)
        {
            PlaceRowOfTiles(i, rowShift);
            if (chunkIndex == 0 && i < 5)
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

    private void PlaceRowOfTiles(int row, int rowShift)
    {
        var tilesInRow = new GameObject[3];

        for (int i = 0; i < 3; i++)
        {
            if ((chunkIndex > 0 || row > 5) && i != 1 && Random.Range(0, 12) == 0) continue;

            var position = new Vector3((i - 1) * 8.66f + rowShift * 4.33f, 0f, row * 7.5f);
            tilesInRow[i] = PlaceTile(position, (row / 10) % 3, row != rowCount - 1, 0.15f * row);
        }

        tileRowList.Add(tilesInRow);

        if (chunkIndex > 0 || row > 5)
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

        PlaceRowOfScenery(row, rowShift);
    }

    private void PlaceRowOfScenery(int row, int rowShift)
    {
        var sceneryObject = PillarTileChunkScenery.GenerateRow(row);
        sceneryObject.transform.parent = transform;
        sceneryObject.transform.eulerAngles = transform.eulerAngles;
        sceneryObject.transform.localPosition = new Vector3(rowShift * 4.33f, 0, row * 7.5f);
    }

    private GameObject PlaceTile(Vector3 position, int type, bool withTrails, float animDelay = 0f)
    {
        var tile = tilePrefabs[type % tilePrefabs.Length];

        var instance = Instantiate(tile, transform);
        instance.GetComponent<AnimatedTexture>().StartAnimation(animDelay);

        instance.transform.localScale = new Vector3(5f, 2.5f, 5f);
        instance.transform.localPosition = position;

        //instance.SetColor(Random.ColorHSV(type* 0.25f, type*0.25f + 0.05f, 0.1f, 0.2f, 0.9f, 1, 1, 1));

        if ((chunkIndex > 0 || position.z > 50) && Random.Range(0, 30) == 0)
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
            var anim = AnimatedTexture.AddToGameObject(instance, texSphere);
            anim.StartAnimation(Random.Range(0f, 2f));
        }
        else
        {
            var anim = AnimatedTexture.AddToGameObject(instance, texCube, 4, 4, 16);
            anim.StartAnimation(Random.Range(0f, 0.5f));
        }

        var rb = instance.AddComponent<Rigidbody>();
        rb.mass = scale * 3;
    }
}
