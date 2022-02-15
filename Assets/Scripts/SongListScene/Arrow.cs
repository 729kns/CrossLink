using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private CanvasRenderer[] crs;
    private bool fade;
    private float fadeRate;
    private int arrowCount;
    // Start is called before the first frame update
    void Start()
    {
        crs = GetComponentsInChildren<CanvasRenderer>();
        fadeRate = 0.2f;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (!fade)
        {
            fadeRate += 0.04f;
            crs[arrowCount].SetAlpha(fadeRate);
            if (fadeRate >= 1)
            {
                fadeRate = 0.2f;
                arrowCount++;
                if (arrowCount > 2)
                {
                    fadeRate = 1;
                    fade = true;
                }
            }
        }
        else
        {
            fadeRate -= 0.02f;
            for (int i = 0; i < crs.Length; i++)
                crs[i].SetAlpha(fadeRate);
            if (fadeRate <= 0.2f)
            {
                arrowCount = 0;
                fadeRate = 0.2f;
                fade = false;
            }
        }
    }
}
