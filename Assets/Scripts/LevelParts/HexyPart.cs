using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HexyPart : MonoBehaviour
{
    public GameObject[] chunkPrefabs;

    public float rowInstantiateDistance = 100f;

    List<HexyTileChunk> chunks = new List<HexyTileChunk>();

    private void Start()
    {
        var nextStartPosition = new Vector3();
        var nextStartRotation = Quaternion.identity;

        for (int i = 0; i < chunkPrefabs.Length; i++)
        {
            var go = Instantiate(chunkPrefabs[i],
            nextStartPosition,
            nextStartRotation) as GameObject;

            var hexChunk = go.GetComponent<HexyTileChunk>();

            nextStartPosition = hexChunk.GetWorldExitPosition();
            nextStartRotation = hexChunk.GetWorldExitRotation();

            chunks.Add(hexChunk);
        }

        ///////
        for(int i = 0; i < chunks[0].rowCount; i++)
        {
            chunks[0].GetRow(i);
        }
    }
}
