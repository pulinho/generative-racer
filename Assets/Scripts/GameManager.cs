using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private static int currentScene = 0;

    public GameObject playerPrefab;
    public PlayerManager[] players;
    public CameraController cameraController;

    private int playersAlive = 0;

    private readonly WaitForSeconds endWait = new WaitForSeconds(2);

    private void Start()
    {
        playersAlive = players.Length;
        
        StartCoroutine(GameLoop());
    }

    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[playersAlive];

        int index = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isAlive)
            {
                targets[index] = players[i].instance.transform;
                index++;
            }
        }
        
        cameraController.targets = targets;
    }

    private void SpawnAllPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].instance =
                Instantiate(playerPrefab, players[i].spawnPoint.position, players[i].spawnPoint.rotation) as GameObject;
            players[i].instance.GetComponent<HoverCarController>().playerNumber = i;
            players[i].Setup();

            // var rb = players[i].instance.GetComponent<Rigidbody>();
            // rb.AddForce(Vector3.forward * 800000);
        }
    }

    public void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Input.GetButton("Reset"))
        {
            SceneManager.LoadScene(++currentScene % 2);
        }
    }
    
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        SceneManager.LoadScene(++currentScene % 2);
    }

    private IEnumerator RoundStarting()
    {
        SpawnAllPlayers();
        SetCameraTargets();
        yield return new WaitForSeconds(1);
        cameraController.InitialGlitch();
    }

    private IEnumerator RoundPlaying()
    {
        while (CountPlayersAlive() > 0)
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        yield return endWait;
    }

    private int CountPlayersAlive()
    {
        int playersAliveNow = 0;
        
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isAlive)
            {
                playersAliveNow++;
            }
        }

        if(playersAlive != playersAliveNow)
        {
            playersAlive = playersAliveNow;
            SetCameraTargets();
        }
        
        return playersAliveNow;
    }
}
