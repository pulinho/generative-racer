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
        instance.GetComponent<HoverCarController>().controlsActivated = true;
    }

    public void SetControllerNumber(int number)
    {
        instance.GetComponent<HoverCarController>().controllerNumber = number;

        if (number < 0)
        {
            Kill(false);
            return;
        }

        isAlive = true;

        instance.SetColor(playerColor);
        instance.transform.Find("PylonWrapper/Pylon").gameObject.SetColor(Color.gray); // todo some beter way
        instance.transform.Find("PylonWrapper/Pylon/Flag").gameObject.SetColor(playerColor);
        instance.transform.Find("PylonWrapper/Pylon2").gameObject.SetColor(Color.gray);
        instance.transform.Find("PylonWrapper/Pylon2/Sail").gameObject.SetColor(Color.white);
    }

    public void Kill(bool withSound = true)
    {
        isAlive = false;
        instance.SetColor(Color.white);
        if(withSound)
        {
            FMODUnity.RuntimeManager.PlayOneShot(deathEvent, transform.position);
        }
    }
}
