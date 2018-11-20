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

    public void InitialGlitch()
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
    }

    private void FixedUpdate()
    {
        FindAveragePosition();
        
        Zoom();
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.SmoothDamp(transform.position, averagePlayerPosition + offset, ref moveVelocity, dampTime);
    }

    virtual protected void Zoom()
    {
        maxPlayerDistance = 10.0f;

        foreach(var target in targets)
        {
            var bias = Camera.main.aspect;
            var biasVec = new Vector3(1 / bias, 1, 1);

            var distance = Vector3.Distance(Vector3.Scale(averagePlayerPosition, biasVec), Vector3.Scale(target.position, biasVec));
            //var distance = Vector3.Distance(averagePlayerPosition, target.position);
            if (distance > maxPlayerDistance)
            {
                maxPlayerDistance = distance;
            }
        }
        
        offset = (new Vector3(0, 2.1f, -1.4f) * maxPlayerDistance);
    }

    virtual protected void FindAveragePosition()
    {
        Vector3 minVector = new Vector3(1000000000, 0, 1000000000);
        Vector3 maxVector = new Vector3(-1000000000, 0, -1000000000);

        foreach(var target in targets)
        {
            if (!target.gameObject.activeSelf)
            {
                continue;
            }
            
            if(target.position.x < minVector.x)
            {
                minVector.x = target.position.x;
            }
            if (target.position.z < minVector.z)
            {
                minVector.z = target.position.z;
            }
            if (target.position.x > maxVector.x)
            {
                maxVector.x = target.position.x;
            }
            if (target.position.z > maxVector.z)
            {
                maxVector.z = target.position.z;
            }
        }

        averagePlayerPosition = minVector + ((maxVector - minVector) / 2);
    }
}
