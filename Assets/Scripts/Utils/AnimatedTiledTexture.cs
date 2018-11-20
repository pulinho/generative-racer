using UnityEngine;
using System.Collections;

class AnimateTiledTexture : MonoBehaviour
{
    public int frameCount = 64; // ideally rows*cols
    public int columns = 8;
    public int rows = 8;
    public float framesPerSecond = 30f;

    //the current frame to display
    private int index = 0;

    void Start()
    {
        StartCoroutine(UpdateTiling());

        //set the tile size of the texture (in UV units), based on the rows and columns
        Vector2 size = new Vector2(1f / columns, 1f / rows);
        GetComponent<Renderer>().sharedMaterial.SetTextureScale("_MainTex", size);
    }

    private IEnumerator UpdateTiling()
    {
        float x = 0f;
        float y = 0f;
        Vector2 offset = Vector2.zero;

        while (true)
        {
            for (int i = rows - 1; i >= 0; i--) // y
            {
                y = (float)i / rows;

                for (int j = 0; j <= columns - 1; j++) // x
                {
                    index++;
                    if (index >= frameCount) break;
                    ///

                    x = (float)j / columns;

                    offset.Set(x, y);

                    GetComponent<Renderer>().sharedMaterial.SetTextureOffset("_MainTex", offset);
                    yield return new WaitForSeconds(1f / framesPerSecond);
                }
                if (index >= frameCount) break; ///
            }
            if (index >= frameCount) index = 0;

            /*if (RunOnce)
            {
                yield break;
            }*/
        }
    }
}
