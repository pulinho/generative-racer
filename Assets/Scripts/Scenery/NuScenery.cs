using System;
using UnityEngine;

public class NuScenery : IScenery
{
    private static Mesh cubeMesh;
    private static int stuffPerRow = 40;

    public GameObject GenerateRow(int row)
    {
        if (cubeMesh == null) cubeMesh = CreateCubeMesh();

        var go = new GameObject();

        var instance = GeneratePillar(row);
        instance.transform.parent = go.transform;
        //instance.transform.localPosition = new Vector3((row % 2 == 0) ? pillarDistance : -pillarDistance, shiftDown, 0);
        //instance.transform.localEulerAngles = new Vector3(0, 0, (row % 2 == 0) ? -45 : -135);

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

        for (int i = 0; i < stuffPerRow; i++)
        {
            var instance = GeneratePillarSlice(i);
            instance.transform.parent = go.transform;
            //if (row % 2 == 0)
            //{
                instance.transform.localEulerAngles = new Vector3(i * 24, (row % 12) * 30, 0);
            //}
        }
        // lolol
        //go.AddComponent<ContinuousRotationY>().StartRotating((0.15f * row) % 2.5f);

        return go;
    }

    private static GameObject GeneratePillarSlice(int sliceIndex)
    {
        var go = new GameObject();

        var meshFilter = go.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = cubeMesh;
        var renderer = go.AddComponent<MeshRenderer>();

        renderer.receiveShadows = false; // ?
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        var width = (stuffPerRow / 1.5f) / (Math.Abs(sliceIndex - stuffPerRow / 2) + 1);

        go.transform.localScale = new Vector3(width, width, width);
        go.transform.localPosition = new Vector3(sliceIndex * 15 - (stuffPerRow / 2 * 15), -30, 0);

        return go;
    }
}
