using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SwitchBTSongList : MonoBehaviour
{
    public GameObject eventSystem;
    public CanvasRenderer[] ringCR, objectCR;
    public CanvasRenderer centerCR, FBCR;
    public Image link1, link2;
    public PackList packList;
    public Material digitRain;
    public DropEmitter dropEmitter;
    public Subtitles subtitles;
    public AudioVisualize audioVisualize;
    public RectTransform[] upperObject;
    public RectTransform leftObject, rightObject;
    public RectTransform ring;
    private AudioBase simpleClick;
    private int state;
    private float fadeRate, centerFadeRate;
    private float scale;
    private Material mat1, mat2;
    private static bool first = true;

    // Start is called before the first frame update
    void Start()
    {
        simpleClick = GetComponent<AudioBase>();
        mat1 = link1.material;
        mat2 = link2.material;
        digitRain.SetTextureScale("_MainTex", Vector2.one);
        digitRain.SetColor("_EmisColor", new Color(0, 1, 0.863f));
        eventSystem.SetActive(false);
        if (first)
        {
            first = false;
            subtitles.enabled = true;
            dropEmitter.enabled = true;
            dropEmitter.StartEmit();
            audioVisualize.ActiveVisualizer();
            fadeRate = 1;
            state = -3;
        }
        else
        {
            Destroy(FBCR.gameObject);
            SwitchBack();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 2:
                if (fadeRate < 0.99f)
                {
                    fadeRate += 0.02f;
                    for (int i = 0; i < objectCR.Length; i++)
                        objectCR[i].SetAlpha(fadeRate);
                    mat1.SetColor("_Color", new Color(1, 1, 1, fadeRate));
                    mat2.SetColor("_Color", new Color(1, 1, 1, fadeRate));
                    leftObject.anchoredPosition -= Vector2.left;
                    rightObject.anchoredPosition -= Vector2.right;
                    for (int i = 0; i < upperObject.Length; i++)
                        upperObject[i].anchoredPosition -= Vector2.up;
                }
                else
                {
                    eventSystem.SetActive(true);
                    state = 0;
                }
                break;
            case 1:
                if (fadeRate < 0.99f)
                {
                    fadeRate += 0.02f;
                    scale -= 0.01f;
                    for (int i = 0; i < ringCR.Length; i++)
                        ringCR[i].SetAlpha(fadeRate);
                    ring.localScale = new Vector2(scale, scale);
                }
                else
                {
                    state = 2;
                    audioVisualize.ActiveVisualizer();
                    dropEmitter.enabled = true;
                    dropEmitter.StartEmit();
                    subtitles.enabled = true;
                    fadeRate = 0;
                    centerCR.gameObject.SetActive(false);
                }
                if (fadeRate > 0.79f)
                {
                    centerFadeRate -= 0.1f;
                    centerCR.SetAlpha(centerFadeRate);
                }
                break;
            case -1:
                if (fadeRate > 0.01f)
                {
                    fadeRate -= 0.02f;
                    for (int i = 0; i < objectCR.Length; i++)
                        objectCR[i].SetAlpha(fadeRate);
                    mat1.SetColor("_Color", new Color(1, 1, 1, fadeRate));
                    mat2.SetColor("_Color", new Color(1, 1, 1, fadeRate));
                    leftObject.anchoredPosition += Vector2.left;
                    rightObject.anchoredPosition += Vector2.right;
                    for (int i = 0; i < upperObject.Length; i++)
                        upperObject[i].anchoredPosition += Vector2.up;
                }
                else
                {
                    packList.ScaleDownCover();
                    fadeRate = 1;
                    centerFadeRate = 0;
                    state = -2;
                    scale = 1;
                }
                break;
            case -2:
                if (fadeRate > 0.01f)
                {
                    fadeRate -= 0.02f;
                    scale += 0.01f;
                    for (int i = 0; i < ringCR.Length; i++)
                        ringCR[i].SetAlpha(fadeRate);
                    ring.localScale = new Vector2(scale, scale);
                }
                else
                {
                    centerCR.SetAlpha(1);
                    SceneManager.LoadScene(1);
                }
                if (fadeRate < 0.21f)
                {
                    centerFadeRate += 0.1f;
                    centerCR.SetAlpha(centerFadeRate);
                }
                break;
            case -3:
                if (fadeRate > 0.01f)
                {
                    fadeRate -= 0.02f;
                    FBCR.SetAlpha(fadeRate);
                }
                else
                {
                    Destroy(FBCR.gameObject);
                    eventSystem.SetActive(true);
                    state = 0;
                }
                break;
        }
    }

    void SwitchBack()
    {
        centerCR.gameObject.SetActive(true);
        centerCR.SetAlpha(1);
        packList.ScaleUpCover();
        fadeRate = 0;
        centerFadeRate = 1;
        scale = 1.5f;
        mat1.SetColor("_Color", new Color(1, 1, 1, fadeRate));
        mat2.SetColor("_Color", new Color(1, 1, 1, fadeRate));
        for (int i = 0; i < objectCR.Length; i++)
            objectCR[i].SetAlpha(fadeRate);
        for (int i = 0; i < ringCR.Length; i++)
            ringCR[i].SetAlpha(fadeRate);
        ring.localScale = new Vector2(scale, scale);
        state = 1;

        leftObject.anchoredPosition += new Vector2(-50, 0);
        rightObject.anchoredPosition += new Vector2(50, 0);
        for (int i = 0; i < upperObject.Length; i++)
            upperObject[i].anchoredPosition += new Vector2(0, 50);
    }

    public void SwitchToList()
    {
        simpleClick.Play();
        eventSystem.SetActive(false);
        centerCR.gameObject.SetActive(true);
        SwitchSceneAnimation.atListScene = true;
        centerCR.SetAlpha(0);
        audioVisualize.CloseVisualizer();
        dropEmitter.StopEmit();
        subtitles.Close();
        fadeRate = 1;
        state = -1;
    }
}
