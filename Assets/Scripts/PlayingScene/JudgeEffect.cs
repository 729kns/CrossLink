using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JudgeEffect : MonoBehaviour
{
    public RectTransform[] layers;
    private Image[] ims;
    public ParticleSystem particle;
    public Sprite iridescent;
    private CanvasRenderer[] crs;
    private float holdTime;
    private Vector3 scaleSpeed = new Vector3(0.09f, 0.09f, 0);
    private float fadeRate = 1;
    private int textState = 2;
    private float flushInterval = 0.02f;
    private bool soundEffect;
    // Start is called before the first frame update
    void Awake()
    {
        crs = GetComponentsInChildren<CanvasRenderer>();
        ims = GetComponentsInChildren<Image>();
        soundEffect = PlayerPrefs.GetInt("Sound Effect", 1) == 1 ? true : false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Launcher.pause)
            return;
        holdTime += Time.deltaTime;
        if (fadeRate > 0)
        {
            layers[2].localScale += scaleSpeed;
            if (holdTime >= 0.2f)
                layers[1].localScale += scaleSpeed;
            if (holdTime >= 0.4f)
            {
                layers[0].localScale += scaleSpeed;
                fadeRate -= 0.06f;
                for (int i = 0; i < crs.Length - 1; i++)
                    crs[i].SetAlpha(fadeRate);
            }
        }
        switch (textState)
        {
            case 1:
                flushInterval -= Time.deltaTime;
                if (flushInterval <= 0)
                {
                    flushInterval = 0.02f;
                    if (crs[3].GetAlpha() == 1)
                        crs[3].SetAlpha(0);
                    else
                        crs[3].SetAlpha(1);
                }
                if (holdTime >= 0.3f)
                {
                    crs[3].SetAlpha(1);
                    textState = 0;
                }
                break;
            case 0:
                if (holdTime >= 0.9f)
                    textState = -1;
                break;
            case -1:
                flushInterval -= Time.deltaTime;
                if (flushInterval <= 0)
                {
                    flushInterval = 0.02f;
                    if (crs[3].GetAlpha() == 1)
                        crs[3].SetAlpha(0);
                    else
                        crs[3].SetAlpha(1);
                }
                break;
        }
        if (holdTime >= 1.2f)
            Destroy(gameObject);
    }

    void SetEffect(EffectData data)
    {
        transform.localPosition = data.pos;
        ParticleSystem.MainModule settings = particle.main;
        for (int i = 0; i < crs.Length - 1; i++)
            crs[i].SetAlpha(fadeRate);
        switch (data.judge)
        {
            case 0:
                textState = 1;
                fadeRate = 0;
                for (int i = 0; i < crs.Length - 1; i++)
                    crs[i].SetAlpha(fadeRate);
                particle.Stop();
                break;
            case 1:
                ims[0].color = new Color(1, 0.55f, 0.55f);
                ims[1].color = new Color(1, 0.37f, 0.37f);
                ims[2].color = new Color(1, 0.55f, 0.55f);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0.37f, 0.37f));
                crs[3].SetAlpha(0);
                if (soundEffect)
                {
                    if (data.end)
                        SoundController.SoundEnd();
                    else
                        SoundController.SoundHit();
                }
                break;
            case 4:
                ims[0].color = new Color(0.51f, 0.88f, 1);
                ims[1].color = new Color(0, 1, 1);
                ims[2].color = new Color(0.51f, 0.88f, 1);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0, 1, 1));
                crs[3].SetAlpha(0);
                if (soundEffect)
                {
                    if (data.end)
                        SoundController.SoundEnd();
                    else
                        SoundController.SoundHit();
                }
                break;
            case 3:
                ims[0].sprite = iridescent;
                ims[1].sprite = iridescent;
                ims[2].sprite = iridescent;
             //   particle.GetComponent<ParticleSystemRenderer>().material = iriParticle;
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 1, 1,0.6f));
                crs[3].SetAlpha(0);
                if (soundEffect)
                {
                    if (data.end)
                        SoundController.SoundEnd();
                    else
                        SoundController.SoundHit();
                }
                break;
            case 2:
                ims[0].color = new Color(1, 0.88f, 0.35f);
                ims[1].color = new Color(1, 0.78f, 0);
                ims[2].color = new Color(1, 0.88f, 0.35f);
                settings.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 0.78f, 0));
                crs[3].SetAlpha(0);
                if (soundEffect)
                {
                    if (data.end)
                        SoundController.SoundEnd();
                    else
                        SoundController.SoundHit();
                }
                break;
        }
    }
}

struct EffectData
{
    public Vector2 pos;
    public int judge;
    public bool end;
    public EffectData(Vector2 setPos, int setJudge,bool setEnd)
    {
        pos = setPos;
        judge = setJudge;
        end = setEnd;
    }
}
