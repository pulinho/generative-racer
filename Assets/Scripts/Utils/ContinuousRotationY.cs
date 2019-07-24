using UnityEngine;
using System.Collections;

public class ContinuousRotationY : MonoBehaviour
{

    private static float rotationsPerMinute = 6f;

    void Update()
    {
        transform.Rotate(0, 6.0f * rotationsPerMinute * Time.deltaTime, 0);
    }
}
