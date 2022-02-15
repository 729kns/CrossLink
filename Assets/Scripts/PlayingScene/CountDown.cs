using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public Image countDigit;
    public Image fill;
    public Sprite[] countDigits;
    public GameObject laucher;
    public RectTransform countStart;
    public GameObject crossLine;
    private int state, countState;
    private float countTime;
    private CanvasRenderer[] crs;
    private RectTransform rt;
    private float fadeRate;
    private float scale;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        crs = GetComponentsInChildren<CanvasRenderer>();
    }

    void OnEnable()
    {
        fadeRate = 0;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
        scale = 1.3f;
        rt.localScale = new Vector2(scale, scale);
        state = 1;
        countState = 2;
        countTime = 1;
        countDigit.sprite = countDigits[countState];
        Instantiate(crossLine, transform).GetComponent<CrossLine>().SerParameter(transform.localPosition, 5, true);
    }


    void Update()
    {
        if (SwitchSceneAnimation.switching)
            return;
        switch (state)
        {
            case 1:
                fadeRate += 0.05f;
                for (int i = 0; i < crs.Length; i++)
                {
                    if (i != 2)
                        crs[i].SetAlpha(fadeRate);
                }
                scale -= 0.015f;
                rt.localScale = new Vector2(scale, scale);
                if (fadeRate >= 1)
                {
                    countTime = 1;
                    fadeRate = 1;
                    state = 0;
                }
                break;
            case 0:
                switch (countState)
                {
                    case 2:
                    case 1:
                    case 0:
                        countTime -= Time.deltaTime;
                        fill.fillAmount = countTime;
                        if (countTime <= 0)
                        {
                            fill.fillAmount = 0;
                            countState--;
                            if (countState >= 0)
                            {
                                countDigit.sprite = countDigits[countState];
                                countTime = 1;
                            }
                            else
                            {
                                scale = 1.3f;
                                countTime = 0.5f;
                            }
                        }
                        break;
                    case -1:
                        fadeRate -= 0.05f;
                        crs[1].SetAlpha(fadeRate);
                        crs[2].SetAlpha(1 - fadeRate);
                        if (scale > 1)
                        {
                            scale -= 0.015f;
                            countStart.localScale = new Vector2(scale, scale);
                        }
                        if (fadeRate <= 0)
                        {
                            fadeRate = 1;
                            countState = -2;
                        }
                        break;
                    case -2:
                        countTime -= Time.deltaTime;
                        if (countTime <= 0)
                            state = -1;
                        break;
                }
                break;
            case -1:
                fadeRate -= 0.05f;
                for (int i = 0; i < crs.Length; i++)
                {
                    if (i != 1)
                        crs[i].SetAlpha(fadeRate);
                }
                scale += 0.015f;
                rt.localScale = new Vector2(scale, scale);
                if (fadeRate <= 0)
                {
                    gameObject.SetActive(false);
                    laucher.SendMessage("Begin");
                }
                break;
        }
    }
}
