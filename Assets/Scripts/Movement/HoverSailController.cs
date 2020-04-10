using UnityEngine;
using System.Collections;

public class HoverSailController : MonoBehaviour
{
    [HideInInspector] public int playerNumber;
    [HideInInspector] public int controllerNumber = -1;
    [HideInInspector] public float defaultRotationY = 0f;
    [HideInInspector] public bool controlsActivated = false;
    [HideInInspector] public int rowIndex;

    Rigidbody body;
    float deadZone = 0.1f;
    public float groundedDrag = 0.5f;
    public float maxVelocity = 50;
    public float hoverForce = 2500;
    public float gravityForce = 2500f;
    public float hoverHeight = 1.5f;
    public float maxAngle = 60f;
    public GameObject[] hoverPoints;
    public GameObject pylon;
    public GameObject projectilePrefab;
    public Transform projectileSpawn;
    public ParticleSystem particleSystem;

    public float forwardAcceleration = 8000f;
    public float reverseAcceleration = 4000f;
    float thrust = 0f;

    public float turnStrength = 1000f;
    float turnValue = 0f;

    //public ParticleSystem[] dustTrails = new ParticleSystem[2];

    int layerMask;

    // Use this for initialization
    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("Vehicle");
        layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        if (!controlsActivated || controllerNumber < 0) return;

        /*thrust = 0.0f;
        float acceleration = 1f;

        if (controllerNumber > 0) {
            var brake = Input.GetAxis("LeftStickVertical" + controllerNumber);
            if (brake < -deadZone) brake = brake * -1.3f;
            else brake = 0;
            acceleration -= brake;
        }
        else {
            acceleration -= Input.GetKey("down") ? 1.3f : 0f;
        }

        //thrust = acceleration * ((acceleration > 0) ? forwardAcceleration : reverseAcceleration);*/

        // Turning
        turnValue = 0.0f;
        float turnAxis = (controllerNumber > 0) ? Input.GetAxis("LeftStickHorizontal" + controllerNumber)
                                                : (Input.GetKey("right") ? 1f : 0f) - (Input.GetKey("left") ? 1f : 0f);

        var rotationY = body.transform.eulerAngles.y;
        var deltaAngle = Mathf.DeltaAngle(defaultRotationY, rotationY);

        if (Mathf.Abs(turnAxis) > deadZone && deltaAngle >= -maxAngle && deltaAngle <= maxAngle)
        {
            turnValue = turnAxis;
        }
        else if (deltaAngle > 0f)
        {
            if (deltaAngle > maxAngle)
            {
                turnValue = -1f;
            }
            else
            {
                turnValue = -deltaAngle / maxAngle;
            }
        }
        else if (deltaAngle < 0f)
        {
            if (deltaAngle < -maxAngle)
            {
                turnValue = 1f;
            }
            else
            {
                turnValue = -deltaAngle / maxAngle;
            }
        }

        targetPylonAngleY = -deltaAngle;


        if (controllerNumber == 0 && Input.GetKey("space"))
        {
            Shoot();
        }
    }

    private float pylonRotationVelocity;
    private float pylonAngleY;
    private float targetPylonAngleY;

    private void RotatePylonSmoothly()
    {
        pylonAngleY = Mathf.SmoothDampAngle(pylonAngleY, targetPylonAngleY, ref pylonRotationVelocity, 0.2f);
        pylon.transform.localEulerAngles = new Vector3(0, pylonAngleY, 0);
    }

    public void FixedUpdate()
    {
        RotatePylonSmoothly();

        RaycastHit hit;
        bool grounded = false;
        for (int i = 0; i < hoverPoints.Length; i++)
        {
            var hoverPoint = hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -Vector3.up, out hit, hoverHeight, layerMask))
            {
                body.AddForceAtPosition(Vector3.up * hoverForce * (1.0f - (hit.distance / hoverHeight)), hoverPoint.transform.position);
                grounded = true;

                // todo: what if not grounded
                // todo: one ray should be enough
                var row = hit.collider.transform.parent/*.parent*/?.GetComponent<TrackRow>()?.rowIndex
                    ?? hit.collider.transform.parent?.parent?.GetComponent<TrackRow>()?.rowIndex ?? rowIndex; // lol no
                if (row > rowIndex)
                    rowIndex = row;
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
            thrust = forwardAcceleration;
        }
        else
        {
            body.drag = 0.2f;
            //thrust /= 9f;
            thrust = forwardAcceleration / 8f;
            turnValue /= 2f;
        }

        /*for (int i = 0; i < dustTrails.Length; i++)
        {
            var emission = dustTrails[i].emission;
            emission.rate = new ParticleSystem.MinMaxCurve(emissionRate);
        }*/

        if (controlsActivated && controllerNumber >= 0) // todo less chujovo or thrust
        {
            // maybe give some weights to each multiplier? Also optimize...
            var pylonToWindAngle = Mathf.DeltaAngle(targetPylonAngleY, pylonAngleY);
            var pylonToWindMultiplier = Mathf.Cos((pylonToWindAngle * Mathf.PI) / 180);

            var pylonToShipFrontMultiplier = Mathf.Cos((pylonAngleY * Mathf.PI) / 180);

            var forceMultiplier = pylonToWindMultiplier * pylonToShipFrontMultiplier;

            body.AddForce(transform.forward * thrust * forceMultiplier);

            var fuelColor = new Color(forceMultiplier, forceMultiplier, forceMultiplier);
            pylon.SetColor(fuelColor);

            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = fuelColor;
            }
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

    bool canShoot = true;

    void Shoot()
    {
        if (canShoot)
        {
            canShoot = false;
            Instantiate(projectilePrefab, projectileSpawn.position, projectileSpawn.rotation);
            StartCoroutine(ShotCoolDown());
        }
    }

    private IEnumerator ShotCoolDown()
    {
        yield return new WaitForSeconds(0.1f);
        canShoot = true;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        // if other is shot, take shots direction
        body.AddForce(-transform.forward);
    }*/
}
