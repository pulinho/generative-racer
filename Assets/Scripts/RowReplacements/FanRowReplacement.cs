using UnityEngine;

public class FanRowReplacement : MonoBehaviour
{
    [HideInInspector] public Texture2D animTex;

    float speed;

    void Start()
    {
        speed = Random.Range(50f, 100f) * (Random.Range(0, 2) * 2 - 1);
        PlaceObstacle();
    }

    void Update()
    {
        transform.Rotate(0f, 0f, speed * Time.deltaTime);
    }

    // a "fan" obstacle
    private void PlaceObstacle()
    {
        /*instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = transform; //
        // instance.transform.eulerAngles = rowtransform.eulerAngles; //
        instance.transform.localPosition = Vector3.zero;// transform.parent.localPosition;

        // var scale = Random.Range(2f, 4f);
        instance.transform.localScale = new Vector3(40f, Random.Range(2f, 4f), 1f);

        instance.transform.Rotate(Vector3.forward * Random.Range(0f, 360f));*/

        var go = new GameObject();
        go.transform.parent = transform; //
        go.transform.localPosition = Vector3.zero;

        var rad = 0f;
        var yMult = (speed > 0) ? -1 : 1;
        for(int i = 0; i < 4; i++)
        {
            var width = 1f + i * 2;
            var y = 0;//(i > 0) ? (1f + (i - 1) * 2) / 2f : 0;
            PlaceCube(go.transform, new Vector3(rad + width / 2f, yMult * y, 0f), width, 2f - i * 0.5f);
            PlaceCube(go.transform, new Vector3(-(rad + width / 2f), -yMult * y, 0f), width, 2f - i * 0.5f);
            rad += width;
        }


        // instance.SetColor(Random.ColorHSV(0, 1, 0.8f, 1.0f, 0.8f, 1, 1, 1));

        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private GameObject PlaceCube(Transform parent, Vector3 position, float width, float delay)
    {
        var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = parent; //
        // instance.transform.eulerAngles = rowtransform.eulerAngles; //
        instance.transform.localPosition = position;// transform.parent.localPosition;

        // var scale = Random.Range(2f, 4f);
        instance.transform.localScale = new Vector3(width, width, 1f);


        var anim = AnimatedTexture.AddToGameObject(instance, animTex, 4, 4, 16);
        anim.StartAnimation(delay);


        return instance;
    }
}
