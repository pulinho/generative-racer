using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkLevelGenerator : MonoBehaviour
{
    public GameObject[] chunkGenerators;
    public GameManager gm;
    public CameraController cameraController;

    private int currentChunkIndex = 0;
    private int topVisitedChunkIndex = 0;

    private Vector3 nextStartPosition = new Vector3();
    private Quaternion nextStartRotation = Quaternion.identity;

    private List<ChunkGeneratorBase> chunks = new List<ChunkGeneratorBase>(); // maybe array suffices?

    private void Awake()
    {
        //Physics.gravity = new Vector3(0, -0.5f, 0);
        for (int i = 0; i < 10; i++)
        {
            GenerateNextChunk();
        }
    }

    public void GenerateNextChunk()
    {
        var go = Instantiate(chunkGenerators[currentChunkIndex % chunkGenerators.Length], 
            nextStartPosition + new Vector3(0, Random.Range(-5f, -2f), 0), 
            nextStartRotation) as GameObject;

        var cg = go.GetComponent<ChunkGeneratorBase>();

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
                //todo slight tilt
                player.instance.GetComponent<HoverCarController>().defaultRotationY += 
                    chunks[topVisitedChunkIndex].rotationY;
                break;
            }
        }
    }
}
