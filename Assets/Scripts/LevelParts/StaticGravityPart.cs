using UnityEngine;
using System.Collections.Generic;

public class StaticGravityPart : MonoBehaviour
{
    public GameObject[] chunkGenerators;
    public GameManager gm;
    public CameraController cameraController;

    private int currentChunkIndex = 0;
    private int topVisitedChunkIndex = 0;

    private Vector3 nextStartPosition = new Vector3();
    private Quaternion nextStartRotation = Quaternion.identity;

    private List<ChunkBase> chunks = new List<ChunkBase>(); // maybe array suffices?

    private void Awake()
    {
        for (int i = 0; i < 12; i++)
        {
            GenerateNextChunk();
        }
    }

    public void GenerateNextChunk()
    {
        var go = Instantiate(chunkGenerators[currentChunkIndex % chunkGenerators.Length], 
            nextStartPosition + new Vector3(0, Random.Range(-5f, -2f), 0), 
            nextStartRotation) as GameObject;

        var cg = go.GetComponent<ChunkBase>();

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
            if(player.currentChunkIndex > topVisitedChunkIndex)
            {
                topVisitedChunkIndex = player.currentChunkIndex;
                cameraController.targetRotationY += chunks[topVisitedChunkIndex].rotationY;
                //todo slight tilt?
                gm.setMusicParameter(topVisitedChunkIndex % 3 + 1);
                break;
            }
        }
    }
}
