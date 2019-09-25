using UnityEngine;

public abstract class ChunkBase : MonoBehaviour
{
    [HideInInspector] public Vector3 worldExitPosition;
    [HideInInspector] public PlayerManager[] players;
    protected int chunkIndex = -1;
    [HideInInspector] public float rotationY; // todo remove

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

            if (player.currentChunkIndex == chunkIndex && IsPlayerInDeadZone(localPosition))
            {
                player.Kill();
                continue;
            }
            if (player.currentChunkIndex == chunkIndex - 1 && IsPlayerOnThisChunk(localPosition))
            {
                player.currentChunkIndex = chunkIndex;
                player.instance.GetComponent<HoverSailController>().defaultRotationY += rotationY;
            }
        }
    }

    abstract public void Init(int chunkIndex);
    abstract protected bool IsPlayerOnThisChunk(Vector3 localPosition);
    abstract protected bool IsPlayerInDeadZone(Vector3 localPosition);
}
