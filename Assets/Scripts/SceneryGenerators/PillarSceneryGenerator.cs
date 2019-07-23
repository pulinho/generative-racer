using System;
using UnityEngine;

public class PillarSceneryGenerator
{
    private static int pillarSliceCount = 10;
    private static float pillarSliceHeight = 15f;
    private static float pillarDistance = 75f;
    private static float shiftDown = -50f;

    public static GameObject GenerateRow(int row)
    {
        var go = new GameObject();

        ///
        if (row % 2 == 0) return go; //////

        var instance = GeneratePillar();
        instance.transform.parent = go.transform;
        instance.transform.eulerAngles = go.transform.eulerAngles; // ???
        instance.transform.localPosition = new Vector3(pillarDistance, shiftDown, 0);

        instance = GeneratePillar();
        instance.transform.parent = go.transform;
        instance.transform.eulerAngles = go.transform.eulerAngles; // ???
        instance.transform.localPosition = new Vector3(-pillarDistance, shiftDown, 0);

        return go;
    }

    private static GameObject GeneratePillar()
    {
        var go = new GameObject();

        for(int i = 0; i < pillarSliceCount; i++)
        {
            var instance = GeneratePillarSlice(i);
            instance.transform.parent = go.transform;
        }

        return go;
    }

    private static GameObject GeneratePillarSlice(int sliceIndex)
    {
        var go = new GameObject(); // todo: w/o collider?

        
        var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        instance.transform.parent = go.transform;

        var width = Math.Abs(pillarSliceCount / 2 - sliceIndex) + 1;

        instance.transform.localScale = new Vector3(width, pillarSliceHeight, width);
        instance.transform.localPosition = new Vector3(0, (sliceIndex - (pillarSliceCount / 2)) * pillarSliceHeight, 0);


        return go;
    }
}
