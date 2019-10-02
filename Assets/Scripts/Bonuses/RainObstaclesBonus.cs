using UnityEngine;

public class RainObstaclesBonus : BonusBase // more like trap...
{
    public override void PerformBonus()
    {
        for(int i = 0; i < 10; i++)
        {
            PlaceObstacle(new Vector3(Random.Range(-40f, 40f), Random.Range(50f, 60f), Random.Range(30f, 50f)));
        }
    }

    private void PlaceObstacle(Vector3 position)
    {
        int type = Random.Range(0, 2);

        var instance = GameObject.CreatePrimitive((type == 0) ? PrimitiveType.Sphere : PrimitiveType.Cube);
        instance.transform.parent = transform.parent; //
        // instance.transform.eulerAngles = rowtransform.eulerAngles; //
        instance.transform.localPosition = position;

        var scale = Random.Range(2f, 4f);
        instance.transform.localScale = new Vector3(scale, scale, scale);

        instance.transform.Rotate(Random.insideUnitSphere * Random.Range(0f, 360f));
        instance.SetColor(Color.black); // Random.ColorHSV(0, 1, 0.8f, 1.0f, 0.8f, 1, 1, 1)

        var rb = instance.AddComponent<Rigidbody>();
        rb.mass = scale * 3;

        rb.AddForce(Vector3.down * 20000f);
    }
}
