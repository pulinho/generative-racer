using UnityEngine;

public class ObstacleMaker : MonoBehaviour
{
    public Texture2D texSphere;
    public Texture2D texCube;

    public void PlaceObstacle(GameObject row, int rowIndex) // maybe pass just transform?
    {
        int type = Random.Range(0, 50);

        if (rowIndex < 10 || type > 2)
        {
            return;
        }

        var instance = GameObject.CreatePrimitive((type == 0) ? PrimitiveType.Sphere : PrimitiveType.Cube);
        instance.transform.parent = row.transform; //
        // instance.transform.eulerAngles = rowtransform.eulerAngles; //
        instance.transform.localPosition = new Vector3(0, 2, 0);

        var scale = Random.Range(2f, 5f);
        instance.transform.localScale = new Vector3(scale, scale, scale);

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
