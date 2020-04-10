using UnityEngine;
using System.Collections;

public class Projectile1Behaviour : MonoBehaviour
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // deal damage
            GameObject.Destroy(gameObject);
        }
    }
}
