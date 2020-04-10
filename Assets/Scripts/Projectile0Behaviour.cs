using UnityEngine;
using System.Collections;

public class Projectile0Behaviour : MonoBehaviour
{
    public float speed = 1f;

    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitiateSelfDestruction());
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition += transform.localRotation * new Vector3(0, 0, speed * Time.deltaTime);
    }

    private IEnumerator InitiateSelfDestruction()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        {
            other.gameObject.GetComponent<Rigidbody>().AddForce(transform.localRotation * Vector3.forward * 1000000f);
            GameObject.Destroy(gameObject);
        }
    }
}
