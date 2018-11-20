using UnityEngine;

public static class GameObject_SetColor
{
    public static void SetColor(this GameObject instance, Color color)
    {
        MeshRenderer[] renderers = instance.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
        {
            renderer.material.color = color;
        }
    }
}
