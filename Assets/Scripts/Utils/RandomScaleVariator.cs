using UnityEngine;

public class RandomScaleVariator : MonoBehaviour
{
    float minScale;
    float maxScale;
    float halfAmplitude;

    Vector3 period;

    // Use this for initialization
    void Start()
    {
        minScale = transform.localScale.x * 0.5f;
        maxScale = transform.localScale.x * 1.5f;
        halfAmplitude = (maxScale - minScale) / 2f;

        period = new Vector3(
            Random.Range(0.5f, 1.5f),
            Random.Range(0.5f, 1.5f),
            Random.Range(0.5f, 1.5f)
        );
    }

    // Update is called once per frame
    void Update()
    {
        var localScale = new Vector3(
            GetScale(period.x),
            GetScale(period.y),
            GetScale(period.z)
        );

        transform.localScale = localScale;
    }

    private float GetScale(float period)
    {
        return minScale + halfAmplitude * (1f + Mathf.Sin(((Time.time % period) / period) * Mathf.PI * 2f));
    }
}
