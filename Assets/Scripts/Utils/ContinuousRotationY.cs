using UnityEngine;
using System.Collections;

public class ContinuousRotationY : MonoBehaviour
{
    private float rotationsPerMinute = 0f;

    public void StartRotating(float delay = 0f, float rpm = 6f)
    {
        StartCoroutine(SetRotations(delay, rpm));
    }

    private IEnumerator SetRotations(float delay = 0f, float rpm = 6f)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        rotationsPerMinute = rpm;
    }

    void Update()
    {
        if(rotationsPerMinute > 0)
        {
            transform.Rotate(0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);
        }
    }
}
