using UnityEngine;

public abstract class ChunkBase : MonoBehaviour
{
    [HideInInspector] public Vector3 worldExitPosition;
    [HideInInspector] public PlayerManager[] players;
    protected int chunkIndex = -1;
    [HideInInspector] public float rotationY;

    protected Vector3 localExitPosition;

    private void FixedUpdate()
    {
        CheckPlayersPositions();
    }

    virtual protected void CheckPlayersPositions()
    {
        foreach (var player in players)
        {
            if (!player.isAlive)
            {
                continue;
            }
            if (player.currentChunkIndex > chunkIndex) // destroy or suspend when all are on next chunks
            {
                continue;
            }

            var localPosition = transform.InverseTransformPoint(player.instance.transform.position);

            if (player.currentChunkIndex == chunkIndex && !IsPlayerAlive(localPosition))
            {
                player.Kill();
                continue;
            }
            if (player.currentChunkIndex < chunkIndex && IsPlayerOnThisChunk(localPosition)) // also if on previous chunk if it's not possible to skip over chunk completely, but who knows
            {
                player.currentChunkIndex = chunkIndex;
                player.instance.GetComponent<HoverSailController>().defaultRotationY += rotationY;
                // IsPlayerAlive ok in next frame I guess
            }
        }
    }

    abstract public void Init(int chunkIndex);
    abstract protected bool IsPlayerOnThisChunk(Vector3 localPosition);
    abstract protected bool IsPlayerAlive(Vector3 localPosition); // ...InDeadZone ?
}
