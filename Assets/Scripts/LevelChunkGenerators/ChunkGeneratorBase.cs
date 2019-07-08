using UnityEngine;
using System.Collections;

public abstract class ChunkGeneratorBase : MonoBehaviour
{
    [HideInInspector] public Vector3 worldExitPosition;
    [HideInInspector] public PlayerManager[] players;

    protected Vector3 localExitPosition;

    private void FixedUpdate()
    {
        CheckPlayersPositions();
    }

    virtual protected void CheckPlayersPositions()
    {
        foreach (var player in players)
        {
            if (player.isAlive && !IsPlayerAlive(transform.InverseTransformPoint(player.instance.transform.position)))
            {
                player.Kill();
            }
        }
    }

    abstract protected bool IsPlayerAlive(Vector3 localPosition);
}
