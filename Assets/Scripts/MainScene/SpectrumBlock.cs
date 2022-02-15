using System.Collections;
using UnityEngine;

public class SpectrumBlock : MonoBehaviour
{
    private CanvasRenderer cr;
    private float fadeRate;
    private int state = 0;

    void Start()
    {
        cr = GetComponent<CanvasRenderer>();
        cr.SetAlpha(0);
    }

    public void Activate()
    {
        state = 1;
        int X = Random.Range(1, AudioVisualize.xEdge);
        int Y = Random.Range(1, AudioVisualize.yEdge);
        transform.localPosition = new Vector2(X * 120, Y * 120) - AudioVisualize.sib;
    }

    void Update()
    {
        switch (state)
        {
            case 1:
                if (fadeRate < 0.98f)
                {
                    fadeRate += 0.04f;
                    cr.SetAlpha(fadeRate);
                }
                else
                    state = -1;
                break;
            case -1:
                if (fadeRate > 0.02f)
                {
                    fadeRate -= 0.04f;
                    cr.SetAlpha(fadeRate);
                }
                else
                    state = 0;
                break;
        }
    }
}
