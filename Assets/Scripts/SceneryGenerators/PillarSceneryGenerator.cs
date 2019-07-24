using System;
using UnityEngine;

public class PillarSceneryGenerator
{
    private static int pillarSliceCount = 15;
    private static float pillarSliceHeight = 14.5f;
    private static float pillarDistance = 75f;
    private static float shiftDown = -20f;
    private static Mesh cubeMesh;

    public static GameObject GenerateRow(int row, bool invert = false)
    {
        if (cubeMesh == null) cubeMesh = CreateCubeMesh();

        var go = new GameObject();

        //var color1 = (row % 4 == 1) ? Color.magenta : Color.cyan;
        //var color2 = (row % 4 == 1) ? Color.cyan : Color.yellow;

        ///
        if (row % 2 == 0)  // return go; //////
        {
            var instance = GeneratePillar();
            instance.transform.parent = go.transform;
            instance.transform.eulerAngles = go.transform.eulerAngles; // ???
            instance.transform.localPosition = new Vector3(pillarDistance, shiftDown, 0);
            instance.transform.localEulerAngles = new Vector3(0, 0, -45);
            // instance.SetColor(invert ? color2 : color1);
        }
        else
        {
            var instance = GeneratePillar(true);
            instance.transform.parent = go.transform;
            instance.transform.eulerAngles = go.transform.eulerAngles; // ???
            instance.transform.localPosition = new Vector3(-pillarDistance, shiftDown, 0);
            instance.transform.localEulerAngles = new Vector3(0, 0, -135);
            // instance.SetColor(invert ? color1 : color2);
        }

        go.SetColor(Color.black);

        return go;
    }

    private static Mesh CreateCubeMesh()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        GameObject.Destroy(go);
        return mesh;
    }

    private static GameObject GeneratePillar(bool shiftY = false)
    {
        var go = new GameObject();

        for(int i = 0; i < pillarSliceCount; i++)
        {
            var instance = GeneratePillarSlice(i);
            instance.transform.parent = go.transform;
            if(shiftY)
            {
                instance.transform.localEulerAngles = new Vector3(0, 45, 0);
            }
        }

        go.AddComponent<ContinuousRotationY>();

        return go;
    }

    private static GameObject GeneratePillarSlice(int sliceIndex)
    {
        var go = new GameObject();

        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = cubeMesh;
        go.AddComponent<MeshRenderer>();

        //var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //instance.transform.parent = go.transform;

        var width = Math.Abs(pillarSliceCount / 2 - sliceIndex) + 1;

        go.transform.localScale = new Vector3(width, pillarSliceHeight, width);
        go.transform.localPosition = new Vector3(0, (sliceIndex - (pillarSliceCount / 2)) * pillarSliceHeight, 0);


        return go;
    }
}
