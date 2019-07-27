using UnityEngine;
using Kino;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform[] targets;
    public float dampTime = 0.2f;

    protected Vector3 offset;
    private Vector3 moveVelocity;
    protected Vector3 averagePlayerPosition;
    protected float maxPlayerDistance;

    private Quaternion rotation;
    private float rotationY;
    public float targetRotationY;
    private float rotationVelocity;

    /*public void InitialGlitch()
    {
        GetComponent<Datamosh>().Glitch();
        StartCoroutine(IncreaseEntropy());
    }

    private IEnumerator IncreaseEntropy()
    {
        var datamosh = GetComponent<Datamosh>();

        while(true)
        {
            yield return new WaitForSeconds(6);
            if (datamosh.entropy < 1f)
            {
                datamosh.entropy += 0.05f;
            }
            datamosh.Glitch();
        }
    }*/

    public void setBackgroundColor(Color color)
    {
        GetComponent<Camera>().backgroundColor = color;
    }

    public void Glitch()
    {
        var datamosh = GetComponent<Datamosh>();
        datamosh.Glitch();
        /*if (datamosh.entropy < 1f)
        {
            datamosh.entropy += 0.05f;
        }*/
    }

    private void FixedUpdate()
    {
        Rotate();

        FindAveragePosition();
        
        Zoom();
        Move();
    }

    private void Rotate()
    { //if not same?
        var old = rotationY;
        rotationY = Mathf.SmoothDampAngle(rotationY, targetRotationY, ref rotationVelocity, 1f); //0.5
        rotation = Quaternion.Euler(0, rotationY, 0);
        transform.RotateAround(transform.position, Vector3.up, rotationY - old);
    }

    private void Move()
    {
        transform.position = Vector3.SmoothDamp(transform.position, 
            Quaternion.Inverse(rotation) * averagePlayerPosition + offset, 
            ref moveVelocity, dampTime);
    }

    private void Zoom()
    {
        maxPlayerDistance = 20.0f;

        foreach (var target in targets)
        {
            var distance = Vector3.Distance(averagePlayerPosition, rotation * target.position);

            if (distance > maxPlayerDistance)
            {
                maxPlayerDistance = distance;
            }
        }

        offset = rotation * new Vector3(0, 1.2f, -1.5f) * maxPlayerDistance;
    }

    virtual protected void FindAveragePosition()
    {
        Vector3 minVector = new Vector3(1000000000, 0, 1000000000);
        Vector3 maxVector = new Vector3(-1000000000, 0, -1000000000);
        float ySum = 0f;

        foreach (var target in targets)
        {
            var rotatedPosition = rotation * target.position;

            if (rotatedPosition.x < minVector.x)
            {
                minVector.x = rotatedPosition.x;
            }
            if (rotatedPosition.z < minVector.z)
            {
                minVector.z = rotatedPosition.z;
            }
            if (rotatedPosition.x > maxVector.x)
            {
                maxVector.x = rotatedPosition.x;
            }
            if (rotatedPosition.z > maxVector.z)
            {
                maxVector.z = rotatedPosition.z;
            }

            ySum += rotatedPosition.y;
        }

        var targetCount = (targets.Length == 0) ? 1 : targets.Length;
        var averageY = ySum / targetCount;

        averagePlayerPosition = minVector + ((maxVector - minVector) / 2) + new Vector3(0, averageY, 0);
    }
}
