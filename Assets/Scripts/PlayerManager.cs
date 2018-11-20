using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [HideInInspector] public bool isAlive;
    // [HideInInspector] public int playerNumber;
    [HideInInspector] public GameObject instance;
    public Color playerColor;
    public Transform spawnPoint;

    public void Setup()
    {
        isAlive = true;

        instance.SetColor(playerColor);
    }

    public void FixedUpdate()
    {
        if(isAlive && instance.transform.position.y < -5)
        {
            isAlive = false;
            instance.SetActive(false);
        }
    }
}
