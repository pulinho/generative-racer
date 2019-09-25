using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TileChunkBase : ChunkBase
{
    public int rowCount = 20;
    public int colCount = 10;
    public float rowDestroyTime = 0.2f;
    public float rowDestroyDelay = 3f;
    public GameObject dirParticleSystem;

    protected List<GameObject[]> tileRowList = new List<GameObject[]>();

    public void StartTileDestruction()
    {
        StartCoroutine(DestroyNextRow());
    }

    private IEnumerator DestroyNextRow()
    {
        yield return new WaitForSeconds(rowDestroyDelay);

        while (tileRowList.Count > 0)
        {
            var lastRow = tileRowList[0];

            for (int i = 0; i < lastRow.Length; i++)
            {
                var tile = lastRow[i];
                if (tile == null) continue;

                var rb = tile.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.mass = 1000;
                rb.useGravity = false;
                var cf = tile.AddComponent<ConstantForce>();
                cf.force = new Vector3(0, -5000, 0); // -4000 ?
                rb.AddTorque(Random.insideUnitSphere * 100000);
                Object.Destroy(tile, 8f);
            }
            tileRowList.RemoveAt(0);

            yield return new WaitForSeconds(rowDestroyTime);
        }
    }

    protected override bool IsPlayerOnThisChunk(Vector3 localPosition)
    {
        if (localPosition.z >= 0 && localPosition.z <= localExitPosition.z
            && localPosition.x > -20 && localPosition.x < 20) // not super accurate
        {
            return true;
        }
        return false;
    }

    protected override bool IsPlayerInDeadZone(Vector3 localPosition)
    {
        if (localPosition.y < -3)
        {
            return true;
        }
        return false;
    }
}
