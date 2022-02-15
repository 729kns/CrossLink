using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrossLine : MonoBehaviour
{
    public RectTransform horizontal, vertical, trackGlow;
    public RectTransform frame;
    private float holdTime;
    private RectTransform rect;
    public CanvasRenderer[] cr;
    public CanvasRenderer glowCR;
    private int state = 1;
    private float scale, frameScale;
    private bool countDown;
    private float glowFadeRate = 1, frameFadeRate = 0;
    private bool glowFade;
    // Start is called before the first frame update
    void Start()
    {
        scale = 0;
        horizontal.sizeDelta = new Vector2(CameraAdjust.screenSize.x, 2);
        vertical.sizeDelta = new Vector2(2, CameraAdjust.screenSize.y);
        horizontal.localScale = new Vector3(scale, 1, 1);
        vertical.localScale = new Vector3(1, scale, 1);
        rect = GetComponent<RectTransform>();
        horizontal.localPosition = new Vector3(-rect.localPosition.x, 0, 0);
        vertical.localPosition = new Vector3(0, -rect.localPosition.y, 0);
        for (int i = 0; i < cr.Length; i++)
            cr[i].SetAlpha(0);
        glowCR.SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Launcher.pause && !countDown || SwitchSceneAnimation.switching)
            return;
        holdTime -= Time.deltaTime;
        if (frameScale > 0.5f)
        {
            frameScale -= 3 * Time.deltaTime;
            frame.localScale = new Vector2(frameScale, frameScale);
        }
        if (glowFade && glowFadeRate>0)
        {
            glowFadeRate -= 0.05f;
            glowCR.SetAlpha(glowFadeRate);
        }
        switch (state)
        {
            case 0:
                if (frameFadeRate < 1)
                {
                    frameFadeRate += 0.025f;
                    cr[1].SetAlpha(frameFadeRate);
                }
                if (holdTime <= 0)
                    state = 2;
                break;
            case 1:
                scale += Time.deltaTime * 2;
                if (scale >= 1)
                {
                    scale = 1;
                    state = 0;
                }
                horizontal.localScale = new Vector3(scale, 1, 1);
                vertical.localScale = new Vector3(1, scale, 1);
                
                cr[0].SetAlpha(scale);
                glowCR.SetAlpha(scale);
                rect.localScale = new Vector3(scale, scale, 1);
                break;
            case 2:
                scale -= Time.deltaTime * 2;
                horizontal.localScale = new Vector3(scale, 1, 1);
                vertical.localScale = new Vector3(1, scale, 1);
                for (int i = 0; i < cr.Length; i++)
                    cr[i].SetAlpha(scale);
                if (!glowFade)
                    glowCR.SetAlpha(scale);
                rect.localScale = new Vector3(scale, scale, 1);
                if (scale <= 0)
                    Destroy(gameObject);
                break;
        }
    }

    public void SerParameter(Vector2 tarPos, float resTime,Vector2 glowDirct)
    {
        transform.SetParent(transform.parent.parent);
        transform.localPosition = tarPos;
        holdTime = resTime;
        frameScale = 0.5f + holdTime * 3;
        frame.localScale = new Vector2(frameScale, frameScale);

        if (glowDirct.y == 0)
            glowCR.SetColor(new Color(1, 0.5f, 0.5f));
        else
            glowCR.SetColor(new Color(0.5f, 1, 1));
        trackGlow.sizeDelta = new Vector2(Screen.currentResolution.width, 16);
        trackGlow.eulerAngles = new Vector3(0, 0, 90 * glowDirct.y);
        trackGlow.localPosition = -trackGlow.sizeDelta.x * 0.5f * glowDirct;
        transform.SetAsFirstSibling();
    }

    public void SerParameter(Vector2 tarPos, float resTime1, float resTime2,Vector2 glowDirct)
    {
        transform.SetParent(transform.parent.parent);
        transform.localPosition = tarPos;
        holdTime = resTime2;
        frameScale = 0.5f + resTime1 * 3;
        frame.localScale = new Vector2(frameScale, frameScale);

        if (glowDirct.y == 0)
            glowCR.SetColor(new Color(1, 0.5f, 0.5f));
        else
            glowCR.SetColor(new Color(0.5f, 1, 1));
        trackGlow.sizeDelta = new Vector2(Screen.currentResolution.width, 16);
        trackGlow.eulerAngles = new Vector3(0, 0, 90 * glowDirct.y);
        trackGlow.localPosition = -trackGlow.sizeDelta.x * 0.5f * glowDirct;
        transform.SetAsFirstSibling();
    }

    public void SerParameter(Vector2 tarPos, float resTime, bool setCountDown)
    {
        transform.SetParent(transform.parent.parent);
        transform.localPosition = tarPos;
        holdTime = resTime;

        if (setCountDown)
        {
            transform.SetAsFirstSibling();
            countDown = true;
            GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
        frame.gameObject.SetActive(false);
        trackGlow.gameObject.SetActive(false);
    }

    public void move(Vector2 pos)
    {
        glowFade = true;
        transform.localPosition = pos;
        horizontal.localPosition = new Vector3(-rect.localPosition.x, 0, 0);
        vertical.localPosition = new Vector3(0, -rect.localPosition.y, 0);
    }
}
