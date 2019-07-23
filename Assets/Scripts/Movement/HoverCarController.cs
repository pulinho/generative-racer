using UnityEngine;
using System.Collections;

public class HoverCarController : MonoBehaviour
{
    [HideInInspector] public int playerNumber;
    [HideInInspector] public int controllerNumber = -1;
    [HideInInspector] public float defaultRotationY = 0f;

    Rigidbody body;
    float deadZone = 0.1f;
    public float groundedDrag = 3f;
    public float maxVelocity = 50;
    public float hoverForce = 1000;
    public float gravityForce = 1000f;
    public float hoverHeight = 1.5f;
    public GameObject[] hoverPoints;

    public float forwardAcceleration = 8000f;
    public float reverseAcceleration = 4000f;
    float thrust = 0f;

    public float turnStrength = 1000f;
    float turnValue = 0f;

    //public ParticleSystem[] dustTrails = new ParticleSystem[2];

    int layerMask;

    bool controlsActivated = false;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;

        StartCoroutine(ActivateControls());
    }

    private IEnumerator ActivateControls()
    {
        yield return new WaitForSeconds(1);
        controlsActivated = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlsActivated || controllerNumber < 0) return;

        thrust = 0.0f;
        float acceleration = (controllerNumber > 0) ? 1f - Input.GetAxis("Brake" + controllerNumber) * 1.5f
                                                    : 1f - (Input.GetKey("down") ? 1f : 0f) * 1.5f; // fwd vs reverse acc here???
        if (acceleration > deadZone)
            thrust = acceleration * forwardAcceleration;
        else if (acceleration < -deadZone)
            thrust = acceleration * reverseAcceleration;

        // Turning
        turnValue = 0.0f;
        float turnAxis = (controllerNumber > 0) ? Input.GetAxis("LeftStickHorizontal" + controllerNumber)
                                                : (Input.GetKey("right") ? 1f : 0f) - (Input.GetKey("left") ? 1f : 0f);

        var rotationY = body.transform.eulerAngles.y;
        var deltaAngle = Mathf.DeltaAngle(defaultRotationY, rotationY);

        if (Mathf.Abs(turnAxis) > deadZone && deltaAngle >= -30f && deltaAngle <= 30f)
        {
            turnValue = turnAxis;
        }
        else if (deltaAngle > 0f)
        {
            if (deltaAngle > 30f)
            {
                turnValue = -1f;
            }
            else
            {
                turnValue = -deltaAngle / 30f;
            }
        }
        else if (deltaAngle < 0f)
        {
            if (deltaAngle < -30f)
            {
                turnValue = 1f;
            }
            else
            {
                turnValue = -deltaAngle / 30f;
            }
        }
    }

    public void FixedUpdate()
    {
        RaycastHit hit;
        bool grounded = false;
        for (int i = 0; i < hoverPoints.Length; i++)
        {
            var hoverPoint = hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -Vector3.up, out hit, hoverHeight, layerMask))
            {
                body.AddForceAtPosition(Vector3.up * hoverForce * (1.0f - (hit.distance / hoverHeight)), hoverPoint.transform.position);
                grounded = true;
            }
            else
            {
                // Self levelling - returns the vehicle to horizontal when not grounded
                if (transform.position.y > hoverPoint.transform.position.y)
                {
                    body.AddForceAtPosition(hoverPoint.transform.up * gravityForce, hoverPoint.transform.position);
                }
                else
                {
                    body.AddForceAtPosition(hoverPoint.transform.up * -gravityForce, hoverPoint.transform.position);
                }
            }
        }

        //var emissionRate = 0;
        if (grounded)
        {
            body.drag = groundedDrag;
            //emissionRate = 10;
        }
        else
        {
            body.drag = 0.1f;
            thrust /= 10f;
            turnValue /= 2f;
        }

        /*for (int i = 0; i < dustTrails.Length; i++)
        {
            var emission = dustTrails[i].emission;
            emission.rate = new ParticleSystem.MinMaxCurve(emissionRate);
        }*/

        if (Mathf.Abs(thrust) > 0)
        {
            body.AddForce(transform.forward * thrust);
        }
        
        if (turnValue > 0)
        {
            body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
        }
        else if (turnValue < 0)
        {
            body.AddRelativeTorque(Vector3.up * turnValue * turnStrength);
        }
        
        if (body.velocity.sqrMagnitude > (body.velocity.normalized * maxVelocity).sqrMagnitude)
        {
            body.velocity = body.velocity.normalized * maxVelocity;
        }
    }
}
