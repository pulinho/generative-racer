using UnityEngine;
using System.Collections.Generic;

public class StaticGravityPart : MonoBehaviour
{
    public GameObject[] chunkPrefabs;
    public GameManager gm;
    public CameraController cameraController;

    private int currentChunkIndex = 0;
    private int topVisitedChunkIndex = -1;

    private Vector3 nextStartPosition = new Vector3();
    private Quaternion nextStartRotation = Quaternion.identity;

    private List<TileChunkBase> chunks = new List<TileChunkBase>(); // maybe array suffices?

    private void Start()
    {
        for (int i = 0; i < 12; i++)
        {
            GenerateNextChunk();
        }
    }

    public void GenerateNextChunk()
    {
        var go = Instantiate(chunkPrefabs[currentChunkIndex % chunkPrefabs.Length], 
            nextStartPosition + new Vector3(0, Random.Range(-5f, -2f), 0), 
            nextStartRotation) as GameObject;

        var cg = go.GetComponent<TileChunkBase>();

        cg.players = gm.players;
        cg.Init(currentChunkIndex++);

        nextStartPosition = cg.worldExitPosition;
        nextStartRotation = cg.transform.rotation;

        chunks.Add(cg);
    }

    private void FixedUpdate()
    {
        foreach(var player in gm.players)
        {
            if(player.IsActive() && player.currentChunkIndex > topVisitedChunkIndex)
            {
                topVisitedChunkIndex = player.currentChunkIndex;
                cameraController.targetRotationY += chunks[topVisitedChunkIndex].rotationY;
                chunks[topVisitedChunkIndex].StartTileDestruction();
                gm.SetMusicParameter(topVisitedChunkIndex % 3 + 1);
                break;
            }
        }
    }
}
