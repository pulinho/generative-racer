using UnityEngine;

public abstract class GridRowGenerator : MonoBehaviour
{
    public bool isPointTop;
    public bool isReversed;

    public int colCount;
    public float sideSize;

    protected float rowWidth;

    protected float colWidth;
    protected float colHeight;

    public Vector3 nextRowPosition = Vector3.zero;
    public Quaternion nextRowRotation = Quaternion.identity;

    public IRowShifter rowShifter;
    protected int shifterShiftX = 0;

    public IGridPatterner patterner;

    public abstract void RecomputeValues();
    public abstract TrackRow PlaceRow(int rowIndex);
}
