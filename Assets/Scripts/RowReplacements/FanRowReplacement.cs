using UnityEngine;

public class FanRowReplacement : MonoBehaviour
{
    // [HideInInspector] public Color color;

    float speed;

    void Start()
    {
        PlaceObstacle();
        speed = Random.Range(50f, 100f) * (Random.Range(0, 2) * 2 - 1);
    }

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }

    // a "fan" obstacle
    private void PlaceObstacle()
    {
        var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = transform; //
        // instance.transform.eulerAngles = rowtransform.eulerAngles; //
        instance.transform.localPosition = Vector3.zero;// transform.parent.localPosition;

        // var scale = Random.Range(2f, 4f);
        instance.transform.localScale = new Vector3(Random.Range(2f, 4f), 40f, 1f);

        instance.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
        instance.SetColor(Random.ColorHSV(0, 1, 0.8f, 1.0f, 0.8f, 1, 1, 1));

        var rb = instance.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }
}
