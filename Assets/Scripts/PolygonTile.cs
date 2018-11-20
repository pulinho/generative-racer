using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
class PolygonTile : MonoBehaviour
{
    public int sideCount = 6;
    public float sideSize = 1f;
    public float height = 0.1f;
    // public PhysicMaterial mat;

    public Texture2D texture;
    public int texRows = 8;
    public int texCols = 8;
    public int texFrameCount = 64;

    private void Awake()
    {
        Generate();
    }

    private void Generate()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null)
        {
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();


        var vertices = new Vector3[sideCount + 1];
        vertices[0] = new Vector3(0, height/2f, 0);

        var angleStep = 360f / sideCount;

        for (int i = 1; i < sideCount + 1; i++)
        {
            vertices[i] = Quaternion.Euler(0, angleStep * i, 0) * new Vector3(0, height / 2f, sideSize);
        }
        
        var meshVertices = new Vector3[sideCount * 3];

        for (int i = 0; i < sideCount; i++)
        {
            meshVertices[i * 3] = vertices[0];
            meshVertices[i * 3 + 1] = vertices[i + 1];
            meshVertices[i * 3 + 2] = vertices[((i + 1) % sideCount) + 1];
        }
        mesh.vertices = meshVertices;
        
        var meshTriangles = new int[sideCount * 3];

        for (int i = 0; i < sideCount * 3; i++)
        {
            meshTriangles[i] = i;
        }
        mesh.triangles = meshTriangles;


        var meshUV = new Vector2[sideCount * 3];
        for (int i = 0; i < sideCount; i++)
        {
            meshUV[i * 3] = new Vector2(0.5f, 0);
            meshUV[i * 3 + 1] = new Vector2(0, 1);
            meshUV[i * 3 + 2] = new Vector2(1, 1);
        }
        mesh.uv = meshUV;


        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


        if(sideCount == 6) // lol
        {
            for (int i = 0; i < 3; i++)
            {
                var go = new GameObject();
                go.transform.parent = gameObject.transform;
                go.transform.localPosition = new Vector3();
                go.transform.localScale = new Vector3(1.73205f, height, 1);
                go.transform.localRotation = Quaternion.Euler(0, 60 * i, 0);

                var boxc = go.AddComponent<BoxCollider>();
                // boxc.material = mat;
            }
        }


        /*var tex = Resources.Load("Textures/tilted_squares") as Texture2D;
        gameObject.GetComponent<Renderer>().material.mainTexture = tex;
        var anim = gameObject.AddComponent(typeof(AnimateTiledTexture)) as AnimateTiledTexture;
        anim.rows = 4;
        anim.columns = 4;
        anim.frameCount = 16;*/

        /*var tex = Resources.Load("Textures/hyperdonut") as Texture2D;
        gameObject.GetComponent<Renderer>().material.mainTexture = tex;
        var anim = gameObject.AddComponent(typeof(AnimateTiledTexture)) as AnimateTiledTexture;
        anim.rows = 5;
        anim.columns = 5;
        anim.frameCount = 20;*/

        /*var tex = Resources.Load("Textures/three_lines") as Texture2D;
        gameObject.GetComponent<Renderer>().material.mainTexture = tex;
        var anim = gameObject.AddComponent(typeof(AnimateTiledTexture)) as AnimateTiledTexture;*/
        
        gameObject.GetComponent<Renderer>().material.mainTexture = texture;
        var anim = gameObject.AddComponent(typeof(AnimateTiledTexture)) as AnimateTiledTexture;
        anim.rows = texRows;
        anim.columns = texCols;
        anim.frameCount = texFrameCount;
    }
}
