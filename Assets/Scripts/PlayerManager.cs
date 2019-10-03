using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [FMODUnity.EventRef]
    public string deathEvent;

    [HideInInspector] public bool isAlive;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public int currentChunkIndex;
    public Color playerColor;
    public Transform spawnPoint;

    public void ActivateControls()
    {
        instance.GetComponent<HoverSailController>().controlsActivated = true;
    }

    public bool IsActive() // todo not so chujovo
    {
        return instance.GetComponent<HoverSailController>().controlsActivated;
    }

    public void SetControllerNumber(int number)
    {
        instance.GetComponent<HoverSailController>().controllerNumber = number;

        if (number < 0)
        {
            Kill(false);
            return;
        }

        isAlive = true;

        ApplyPlayerColor();
    }

    public void Kill(bool withSound = true)
    {
        isAlive = false;
        instance.SetColor(Color.white);

        /*if(withSound) // uhhhhhhhhhhh
        {
            instance.GetComponent<Rigidbody>().isKinematic = true;
        }*/


        if (withSound)
        {
            FMODUnity.RuntimeManager.PlayOneShot(deathEvent, transform.position);
        }
    }

    public void Respawn(Vector3 position)
    {
        instance.transform.position = position;
        ApplyPlayerColor();
        isAlive = true;

        // reset forces
        var rb = instance.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.isKinematic = false;
    }

    void ApplyPlayerColor()
    {
        instance.SetColor(playerColor);
        instance.transform.Find("PylonWrapper/Pylon").gameObject.SetColor(Color.gray); // todo some beter way
        instance.transform.Find("PylonWrapper/Pylon/Flag").gameObject.SetColor(playerColor);
        instance.transform.Find("PylonWrapper/Pylon2").gameObject.SetColor(Color.gray);
        instance.transform.Find("PylonWrapper/Pylon2/Sail").gameObject.SetColor(Color.white);
    }
}
