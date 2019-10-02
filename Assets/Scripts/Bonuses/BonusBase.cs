using UnityEngine;

public abstract class BonusBase : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // todo check if other is player
        // also animate
        if(other.gameObject.layer == LayerMask.NameToLayer("Vehicle"))
        {
            PerformBonus();
            GameObject.Destroy(gameObject);
        }
    }

    public abstract void PerformBonus();
}
