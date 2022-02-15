using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwitchBTMain : MonoBehaviour
{
    public GameObject eventSystem;
    public SongList songList;
    public InfoPanel infoPanel;
    public NodeNet nodeNet;
    public CanvasRenderer center;
    public CanvasRenderer[] crs;
    public RectTransform[] leftObjects;
    public RectTransform bottomObject;
    public Transform particleRoot;
    public Material digitRain;
    public Transform infos;
    private float fadeRate = 1, centerFadeRate;
    private ParticleSystem[] particles;
    private int switchState;
    private Material infoMat;
    private Color pColor = new Color(0, 1, 0.863f);
    private float tiling = 1;
    // Start is called before the first frame update
    void Start()
    {
        particles = particleRoot.GetComponentsInChildren<ParticleSystem>();
        digitRain.SetTextureScale("_MainTex", Vector2.one);
        digitRain.SetColor("_EmisColor", pColor);
        infoMat = infos.GetComponent<Image>().material;
        if (SwitchSceneAnimation.atListScene)
        {
            for (int i = 0; i < leftObjects.Length; i++)
                leftObjects[i].localPosition -= new Vector3(100, 0, 0);
            bottomObject.localPosition -= new Vector3(0, 50, 0);
            fadeRate = 0;
            centerFadeRate = 1;
            infoMat.SetColor("_Color", new Color(1, 1, 1, fadeRate));
            FloatingCoverGenerater.close = true;
            for (int i = 0; i < crs.Length; i++)
                crs[i].SetAlpha(fadeRate);
            eventSystem.SetActive(false);
            center.gameObject.SetActive(true);
        }
        else
        {
            for (int i = 0; i < particles.Length; i++) {
                ParticleSystem.MainModule m = particles[i].main;
                m.prewarm = true;
            }
        }
        for (int i = 0; i < particles.Length; i++)
            particles[i].Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (NodeNet.skipFrame <= 0 && switchState != -1)
        {
            if (centerFadeRate > 0.1f)
            {
                centerFadeRate -= 0.2f;
                center.SetAlpha(centerFadeRate);
            }
            else
                center.gameObject.SetActive(false);
        }
        switch (switchState)
        {
            case 1:
                if (fadeRate < 0.99f)
                {
                    fadeRate += 0.02f;
                    infoMat.SetColor("_Color", new Color(1, 1, 1, fadeRate));
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                    for (int i = 0; i < leftObjects.Length; i++)
                        leftObjects[i].localPosition += new Vector3(2, 0, 0);
                    bottomObject.localPosition += new Vector3(0, 1, 0);
                }
                else
                {
                    switchState = 0;
                    eventSystem.SetActive(true);
                    infoPanel.InitializePanel();
                }
                break;
            case -1:
                if (fadeRate > 0.01f)
                {
                    fadeRate -= 0.02f;
                    infoMat.SetColor("_Color", new Color(1, 1, 1, fadeRate));
                    for (int i = 0; i < crs.Length; i++)
                    {
                        crs[i].SetAlpha(fadeRate);
                    }
                }
                center.SetAlpha(NodeNet.freezeCount / 30f);
                for (int i = 0; i < leftObjects.Length; i++)
                {
                    leftObjects[i].localPosition -= new Vector3(2, 0, 0);
                }
                bottomObject.localPosition -= new Vector3(0, 1, 0);
                if (tiling > 0)
                {
                    tiling -= 0.01f;
                    digitRain.SetTextureScale("_MainTex", new Vector2(tiling, tiling));
                    pColor -= new Color(0, 0.01f, 0.00863f);
                    digitRain.SetColor("_EmisColor", pColor);
                }
                break;
        }
    }

    public void SceneEnter()
    {
        songList.CreateList();
        FloatingCoverGenerater.close = false;
        fadeRate = 0;
        switchState = 1;
    }

    public void StartAnimation()
    {
        songList.CloseList();
        infoPanel.ClosePanel();
        center.gameObject.SetActive(true);
        center.SetAlpha(0);
        nodeNet.NodeCentralize();
        for (int i = 0; i < particles.Length; i++)
            particles[i].Stop();
        FloatingCoverGenerater.close = true;
        switchState = -1;
    }
}
