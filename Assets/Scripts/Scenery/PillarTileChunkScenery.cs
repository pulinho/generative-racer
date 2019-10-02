using System;
using System.Collections;
using UnityEngine;

public class PillarTileChunkScenery
{
    private static int pillarSliceCount = 15;
    private static float pillarSliceHeight = 15f;
    private static float pillarDistance = 75f;
    private static float shiftDown = -20f;
    private static Mesh cubeMesh;

    public static GameObject GenerateRow(int row)
    {
        if (cubeMesh == null) cubeMesh = CreateCubeMesh();

        var go = new GameObject();

        var instance = GeneratePillar(row);
        instance.transform.parent = go.transform;
        instance.transform.localPosition = new Vector3((row % 2 == 0) ? pillarDistance : -pillarDistance, shiftDown, 0);
        instance.transform.localEulerAngles = new Vector3(0, 0, (row % 2 == 0) ? -45 : -135);

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

    private static GameObject GeneratePillar(int row)
    {
        var go = new GameObject();

        for(int i = 0; i < pillarSliceCount; i++)
        {
            var instance = GeneratePillarSlice(i);
            instance.transform.parent = go.transform;
            if(row % 2 == 0)
            {
                instance.transform.localEulerAngles = new Vector3(0, 45, 0);
            }
        }

        go.AddComponent<ContinuousRotationY>().StartRotating((0.15f * row) % 2.5f);

        return go;
    }

    private static GameObject GeneratePillarSlice(int sliceIndex)
    {
        var go = new GameObject();

        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = cubeMesh;
        var renderer = go.AddComponent<MeshRenderer>();


        renderer.receiveShadows = false; // ?
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // ??


        //var instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //instance.transform.parent = go.transform;

        var width = Math.Abs(pillarSliceCount / 2 - sliceIndex) + 2;

        go.transform.localScale = new Vector3(width, pillarSliceHeight, width);
        go.transform.localPosition = new Vector3(0, (sliceIndex - (pillarSliceCount / 2)) * pillarSliceHeight, 0);


        return go;
    }
}
