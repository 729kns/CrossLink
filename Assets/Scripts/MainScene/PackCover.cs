using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PackCover : MonoBehaviour
{
    public Image cover;
    private CanvasRenderer[] crs;
    private Button b;
    private RectTransform rt;
    private int coverCount;
    private float fadeRate;
    private int state;

    void Awake()
    {
        crs = GetComponentsInChildren<CanvasRenderer>();
        rt = GetComponent<RectTransform>();
        b = GetComponent<Button>();
    }

    void Update()
    {
        switch (state)
        {
            case 1:
                if (fadeRate < 0.99f)
                {
                    fadeRate += 0.02f;
                    //   cr.SetAlpha(fadeRate);
                    //   mat.SetFloat("_Alpha", fadeRate);
                    rt.localScale = new Vector2(fadeRate, fadeRate);
                }
                else
                    state = 0;
                break;
            case -1:
                if (fadeRate > 0.01f)
                {
                    fadeRate -= 0.02f;
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                    rt.localScale = new Vector2(fadeRate, fadeRate);
                }
                else
                    state = 0;
                break;
        }
    }

    public void SetCover(string coverName,int count)
    {
        cover.sprite = Resources.Load<Sprite>("Main Cover/" + coverName);
        coverCount = count;
        rt.anchoredPosition = new Vector2(CameraAdjust.screenSize.x * 0.5f + 1280 * count, 0);
    }

    public void Click()
    {
        transform.parent.parent.parent.parent.SendMessage("SwitchToList");
    }

    public void SetAlpha(float contentPos)
    {
        contentPos = rt.anchoredPosition.x - CameraAdjust.screenSize.x * 0.5f - contentPos;
        if (contentPos < 0)
            contentPos = -contentPos;
        fadeRate = 1.1f - contentPos * 0.001f;
        if (fadeRate >= 1)
            b.enabled = true;
        else
            b.enabled = false;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
    }

    public void ScaleDown()
    {
        state = -1;
        fadeRate = 1;
    }

    public void ScaleUp()
    {
        state = 1;
        fadeRate = 0;
    }
}
