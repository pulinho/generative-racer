using UnityEngine;
using System.Collections;

public class Boss0Controller : MonoBehaviour
{
    public GameManager gm;

    float xPosExtra = 0f;

    public GameObject projectilePrefab;
    public Transform[] projectileSpawns;

    // Use this for initialization
    void Start()
    {
        //transform.position = gm.playersCenter.position + new Vector3(0, 0, 50);
    }

    // Update is called once per frame
    void Update() // not fixed??
    {
        //xPosExtra = (((int)(Time.fixedTime / 4f)) % 2 == 0) ? 50 : -50;
        xPosExtra = Mathf.Sin(Time.fixedTime) * 50f;
        // Vector3.SmoothDamp? also maybe not deltaTime
        transform.position = Vector3.Lerp(transform.position, gm.bossTarget.position + new Vector3(xPosExtra, 0, 60), /*0.5f * */Time.deltaTime);

        Shoot();
    }

    bool canShoot = true;

    void Shoot()
    {
        if (canShoot)
        {
            canShoot = false;
            foreach(var spawn in projectileSpawns)
            {
                Instantiate(projectilePrefab, spawn.position, spawn.rotation);
            }
            StartCoroutine(ShotCoolDown());
        }
    }

    private IEnumerator ShotCoolDown()
    {
        yield return new WaitForSeconds(1f);
        canShoot = true;
    }
}
