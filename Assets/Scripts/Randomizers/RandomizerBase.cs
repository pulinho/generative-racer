using UnityEngine;

public abstract class RandomizerBase : MonoBehaviour
{
    public abstract TrackRow GetRow(int rowIndex);
}
