using UnityEngine;
using System.Collections;

public class ChunkLevelGenerator : MonoBehaviour
{
    public GameObject[] chunkGenerators;
    public GameManager gm;
    public CameraController cameraController;

    private int index = 0;

    private Vector3 nextStartPosition = new Vector3();
    private Quaternion nextStartRotation = Quaternion.identity;

    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            GenerateNextChunk();
        }
    }

    public void GenerateNextChunk() // rotation affect camera?
    {
        var go = Instantiate(chunkGenerators[index++ % chunkGenerators.Length], 
            nextStartPosition + new Vector3(0, -5, 0), 
            nextStartRotation) as GameObject;

        var cg = go.GetComponent<ChunkGeneratorBase>();
        nextStartPosition = cg.worldExitPosition;
        nextStartRotation = cg.transform.rotation;

        cg.players = gm.players;
    }
}
