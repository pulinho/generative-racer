using UnityEngine;

public class BonusMaker
{
    public static void PlaceBonus(GameObject row, int rowIndex)
    {
        if (rowIndex < 10)
        {
            return;
        }

        var instance = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        instance.transform.parent = row.transform; //
        // instance.transform.eulerAngles = rowtransform.eulerAngles; //
        instance.transform.localPosition = new Vector3(0, 2, 0);

        var scale = 2f;
        instance.transform.localScale = new Vector3(scale, scale, scale);

        // instance.transform.Rotate(Vector3.up * Random.Range(0f, 360f));
        instance.SetColor(Color.green); // Random.ColorHSV(0, 1, 0, 0.1f, 0.9f, 1, 1, 1)

        /*MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();
        foreach (var renderer in renderers)
        {
            renderer.receiveShadows = false;
        }*/

        var collider = instance.GetComponent<Collider>();
        collider.isTrigger = true;

        instance.AddComponent<RainObstaclesBonus>();
        instance.AddComponent<RandomScaleVariator>();
    }
}
