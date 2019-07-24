using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [HideInInspector] public bool isAlive;
    // [HideInInspector] public int playerNumber;
    [HideInInspector] public GameObject instance;
    [HideInInspector] public int currentChunkIndex;
    public Color playerColor;
    public Transform spawnPoint;

    public void Setup()
    {
        isAlive = true;
    }

    public void SetControllerNumber(int number)
    {
        instance.GetComponent<HoverCarController>().controllerNumber = number;

        if (number < 0)
        {
            instance.SetColor(Color.white);
            return;
        }

        instance.SetColor(playerColor);
        instance.transform.Find("PylonWrapper/Pylon").gameObject.SetColor(Color.gray); // todo some beter way
        instance.transform.Find("PylonWrapper/Pylon/Sail").gameObject.SetColor(Color.white);
        instance.transform.Find("PylonWrapper/Pylon/SmallSail").gameObject.SetColor(playerColor);
    }

    public void Kill()
    {
        isAlive = false;
        instance.SetActive(false);
    }

    /*public void FixedUpdate()
    {
        if(isAlive && instance.transform.position.y < -5)
        {
            isAlive = false;
            instance.SetActive(false);
        }
    }*/
}
