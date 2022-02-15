using System.Collections;
using UnityEngine;

public class FCAnimation : MonoBehaviour
{
    private CanvasRenderer[] crs;
    private int state;
    private float fadeRate;
    private float timeCount, flushInterval = 0.02f;
    // Start is called before the first frame update
    void Awake()
    {
        crs = GetComponentsInChildren<CanvasRenderer>();
    }

    void OnEnable()
    {
        state = 1;
        fadeRate = 0;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 4:
                timeCount -= Time.deltaTime;
                flushInterval -= Time.deltaTime;
                if (flushInterval <= 0)
                {
                    flushInterval = 0.02f;
                    if (crs[3].GetAlpha() == 1)
                        crs[3].SetAlpha(0);
                    else
                        crs[3].SetAlpha(1);
                }
                if (timeCount <= 0)
                {
                    crs[3].SetAlpha(1);
                    timeCount = 1.4f;
                    state = 0;
                }
                break;
            case 3:
                if (fadeRate < 0.95f)
                {
                    fadeRate += 0.1f;
                    crs[0].SetAlpha(fadeRate);
                }
                else
                {
                    timeCount = 0.3f;
                    state = 4;
                }
                break;
            case 2:
                if (fadeRate < 0.95f)
                {
                    fadeRate += 0.1f;
                    crs[1].SetAlpha(fadeRate);
                }
                else
                {
                    fadeRate = 0;
                    state = 3;
                }
                break;
            case 1:
                if (fadeRate < 0.95f)
                {
                    fadeRate += 0.1f;
                    crs[2].SetAlpha(fadeRate);
                }
                else
                {
                    fadeRate = 0;
                    state = 2;
                }
                break;
            case 0:
                timeCount -= Time.deltaTime;
                if (timeCount <= 0)
                {
                    timeCount = 0.3f;
                    state = -1;
                }
                break;
            case -1:
                timeCount -= Time.deltaTime;
                flushInterval -= Time.deltaTime;
                if (flushInterval <= 0)
                {
                    flushInterval = 0.02f;
                    if (crs[3].GetAlpha() == 1)
                        crs[3].SetAlpha(0);
                    else
                        crs[3].SetAlpha(1);
                }
                if (timeCount <= 0)
                {
                    crs[3].SetAlpha(0);
                    fadeRate = 1;
                    state = -2;
                }
                break;
            case -2:
                if (fadeRate > 0.05f)
                {
                    fadeRate -= 0.1f;
                    crs[0].SetAlpha(fadeRate);
                }
                else
                {
                    fadeRate = 1;
                    state = -3;
                }
                break;
            case -3:
                if (fadeRate > 0.05f)
                {
                    fadeRate -= 0.1f;
                    crs[1].SetAlpha(fadeRate);
                }
                else
                {
                    fadeRate = 1;
                    state = -4;
                }
                break;
            case -4:
                if (fadeRate > 0.05f)
                {
                    fadeRate -= 0.1f;
                    crs[2].SetAlpha(fadeRate);
                }
                else
                {
                    gameObject.SetActive(false);
                }
                break;
        }
    }
}
