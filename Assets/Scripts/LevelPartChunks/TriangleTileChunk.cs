using UnityEngine;

public class TriangleTileChunk : TileChunkBase
{
    private float newestRowShift = 0;

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

    private float leftEdge = -20f;
    private float rightEdge = 20f;

    private void PlaceRowOfTiles(int row)
    {
        var tilesInRow = new GameObject[1];

        
        tilesInRow[0] = PlaceTile(new Vector3(newestRowShift * 5, 0f, row * 10), row % 4,
            row != rowCount - 1);


        tileRowList.Add(tilesInRow);

        if (chunkIndex == 0 && row < 3)
        {
            return;
        }

        if (row == rowCount - 1)
        {
            localExitPosition = new Vector3(newestRowShift * 5f, 0f, rowCount * 10f);
            worldExitPosition = transform.TransformPoint(localExitPosition);
            return;
        }

        // PlaceRowOfScenery(row, newestRowShift);

        newestRowShift += Random.Range(-3, 4) % 2;
    }

    /*private void PlaceRowOfScenery(int row, int rowShift)
    {
        var sceneryObject = PillarTileChunkScenery.GenerateRow(row);
        sceneryObject.transform.parent = transform;
        sceneryObject.transform.eulerAngles = transform.eulerAngles;
        sceneryObject.transform.localPosition = new Vector3(rowShift * 5, 0, row * 10f);
    }*/

    private GameObject PlaceTile(Vector3 position, int type, bool withTrails)
    {
        var instance = GetNewTriangle();
        instance.transform.parent = transform;
        instance.transform.eulerAngles = transform.eulerAngles;

        instance.transform.localScale = new Vector3(10, 1, 10);
        instance.transform.localPosition = position;

        instance.SetColor(Random.ColorHSV(type * 0.25f, type * 0.25f + 0.05f, 0.1f, 0.2f, 0.9f, 1, 1, 1));

        if (withTrails)
        {
            Instantiate(dirParticleSystem, instance.transform);
        }

        return instance;
    }

    private static GameObject GetNewTriangle()
    {
        var go = new GameObject();

        go.AddComponent<MeshRenderer>();

        var meshFilter = go.AddComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return null;
        }

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();


        var vertices = new Vector3[3];
        vertices[0] = new Vector3(0, 0, -1);
        vertices[1] = new Vector3(-3, 0, 1);
        vertices[2] = new Vector3(3, 0, 1);

        
        mesh.vertices = vertices;

        var meshTriangles = new int[3];
        for (int i = 0; i < 3; i++)
        {
            meshTriangles[i] = i;
        }
        mesh.triangles = meshTriangles;


        var meshUV = new Vector2[3];
        meshUV[0] = new Vector2(0.5f, 0);
        meshUV[1] = new Vector2(0, 1);
        meshUV[2] = new Vector2(1, 1);
        mesh.uv = meshUV;


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


        go.AddComponent<MeshCollider>();
        return go;
    }
}
