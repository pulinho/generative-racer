using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [FMODUnity.EventRef]
    public string startEvent;

    [FMODUnity.EventRef]
    public string sheppardEvent;

    [FMODUnity.EventRef]
    public string musicEvent;
    static FMOD.Studio.EventInstance? music = null;

    private static int[] controllerNumber;
    private static int playersConnected = 0;
    private static int currentScene = 0;

    public GameObject playerPrefab;
    public PlayerManager[] players;
    public CameraController cameraController;

    private int playersAlive = 0;

    private readonly WaitForSeconds endWait = new WaitForSeconds(2);

    private void Start()
    {
        SetupSound();

        if (controllerNumber == null)
        {
            controllerNumber = new int[] { -1, -1, -1, -1 }; // todo dynamic length
        }
        
        StartCoroutine(GameLoop());
    }

    private void SetupSound()
    {
        if (music == null)
        {
            music = FMODUnity.RuntimeManager.CreateInstance(musicEvent);
            music?.start();

            // FMODUnity.RuntimeManager.PlayOneShot(sheppardEvent);
        }

        music?.setParameterValueByIndex(0, 1f);
    }

    public void SetMusicParameter(float value)
    {
        music?.setParameterValueByIndex(0, value);

        // make separate
        cameraController.Glitch();
        cameraController.SetBackgroundColor(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
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
            players[i].SetControllerNumber(controllerNumber[i]);
        }
    }

    public void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        if (Input.GetButton("Back"))
        {
            LoadNextScene();
        }

        for (int i = 1; i < players.Length + 1; i++)
        {
            if (Input.GetButtonDown("Start" + i))
            {
                AddNewController(i);
            }
        }
        // keyboard
        if (Input.GetKeyDown("return"))
        {
            AddNewController(0);
        }
    }

    public void FixedUpdate()
    {
        if(playersAlive != CountPlayersAlive())
        {
            SetCameraTargets();
        }
    }

    private void AddNewController(int number)
    {
        // disconnect
        for (int i = 0; i < controllerNumber.Length; i++)
        {
            if (controllerNumber[i] == number)
            {
                controllerNumber[i] = -1;
                players[i].SetControllerNumber(-1);
                //CountPlayersAlive();
                playersConnected--;
                return;
            }
        }
        // connect
        for (int i = 0; i < controllerNumber.Length; i++)
        {
            if (controllerNumber[i] < 0)
            {
                controllerNumber[i] = number;
                players[i].SetControllerNumber(number);
                //CountPlayersAlive();
                playersConnected++;
                break;
            }
        }
    }
    
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(++currentScene % SceneManager.sceneCountInBuildSettings);
    }

    private IEnumerator RoundStarting()
    {
        SpawnAllPlayers();

        while (playersAlive == 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(2);

        // cameraController.InitialGlitch();
        FMODUnity.RuntimeManager.PlayOneShot(startEvent);

        foreach (var player in players)
        {
            if(player.isAlive)
            {
                player.ActivateControls();
            }
        }
    }

    private IEnumerator RoundPlaying()
    {
        while (playersAlive > 0) // (playersAlive > 1 || (playersConnected < 2 && playersAlive > 0))
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        // music?.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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

        playersAlive = playersAliveNow;
        return playersAliveNow;
    }
}
