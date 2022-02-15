using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public RectTransform comboRT;
    public RectTransform[] upperItems;
    public GameObject settlePanel;
    public static GameObject judgeEffect;
    public Text scores, combos, integrityText;
    public Image integrityFill;
    public static bool reduction;
    public ParticleSystem[] particles;
    private ParticleSystem.MainModule m;
    private bool settlement;
    private static Transform playground;
    private static float scoreUnit, unit, integrityUnit;
    private static int combo;
    private float comboFadeRate;
    private float comboSize;
    private static int sizeState;
    private static bool increment;

    private static int step;
    private static bool comboFade;
    private static bool movement;
    private static int comboState;
    private static GameObject FC;
    public static float lastScore, score, lastIntegrity, integrity;
    public static int maxCombo;
    public static int perfect, intact, defective, miss;

    // Start is called before the first frame update
    void Start()
    {
        FC = GameObject.FindGameObjectWithTag("FC");
        FC.SetActive(false);
        judgeEffect = (GameObject)Resources.Load("Prefabs/Note Effect");
        playground = transform.Find("Playground");
    }

    void OnEnable()
    {
        scoreUnit = 1000000f / Launcher.totalNote;
        score = 0;
        comboFadeRate = 0;
        combos.color = new Color(1, 1, 1, comboFadeRate);
        comboSize = 1;
        comboRT.localScale = new Vector2(comboSize, comboSize);
        combo = 0;
        maxCombo = 0;
        perfect = 0;
        intact = 0;
        defective = 0;
        miss = 0;
        reduction = false;
        comboFade = false;
        sizeState = 0;
        movement = false;
        comboState = 0;
        increment = PlayerPrefs.GetInt("Integrity Setting", 1) == 1 ? true : false;
        if (increment)
        {
            integrity = 0;
            integrityText.text = "00.00";
            integrityFill.color = new Color(0, 0.859f, 0.843f);
        }
        else
        {
            integrity = 1;
            integrityText.text = "100.0";
            integrityFill.color = new Color(1, 0.75f, 0);
        }
        integrityFill.fillAmount = integrity;
    }

    // Update is called once per frame
    void Update()
    {
        if (settlement)
        {
            if (step < 10)
            {
                step++;
                for (int i = 0; i < upperItems.Length; i++)
                    upperItems[i].localPosition = new Vector2(upperItems[i].localPosition.x, upperItems[i].localPosition.y + 10);
            }
            else
                settlePanel.SetActive(true);
        }
        if (comboFade && comboFadeRate > 0)
        {
            comboFadeRate -= 0.1f;
            combos.color = new Color(1, 1, 1, comboFadeRate);
            comboRT.localPosition = new Vector2(comboRT.localPosition.x, comboRT.localPosition.y - 3);
        }
        if (Launcher.pause && !reduction)
            return;
        if (movement)
        {
            if (Time.frameCount % 2 == 0)
            {
                lastScore += unit;
                scores.text = ((int)(lastScore + 0.5f)).ToString("D7");
                lastIntegrity += integrityUnit + 0.000005f;
                integrityFill.fillAmount = lastIntegrity;
                if (lastIntegrity < 1)
                {
                    integrityText.text = (lastIntegrity * 100).ToString("F2");
                    integrityFill.color = new Color(0, 0.859f, 0.843f);
                }
                else
                {
                    integrityText.text = "100.0";
                    integrityFill.color = new Color(1, 0.75f, 0);
                }
                step++;
                if (step == 7)
                    movement = false;
            }
        }
        switch (comboState)
        {
            case -1:
                if (comboFadeRate <= 0)
                    comboState = 0;
                else
                {
                    comboFadeRate -= 0.1f;
                    combos.color = new Color(1, 1, 1, comboFadeRate);
                }
                comboRT.localPosition = new Vector2(comboRT.localPosition.x, comboRT.localPosition.y - 3);
                break;
            case 0:
                break;
            case 1:
                combos.text = combo.ToString("D3");
                if (comboFadeRate >= 1)
                    comboState = 0;
                else
                {
                    comboFadeRate += 0.1f;
                    combos.color = new Color(1, 1, 1, comboFadeRate);
                }
                comboRT.localPosition = new Vector2(0, 50);
                break;
        }
        switch (sizeState)
        {
            case 1:
                if (comboSize >= 1.1f)
                    sizeState = -1;
                else
                {
                    comboSize += 0.02f;
                    comboRT.localScale = new Vector2(comboSize, comboSize);
                }
                break;
            case 0:
                break;
            case -1:
                if (comboSize <= 1)
                    sizeState = 0;
                else
                {
                    comboSize -= 0.02f;
                    comboRT.localScale = new Vector2(comboSize, comboSize);
                }
                break;
        }
    }

    public void Settle()
    {
        settlement = true;
        comboFade = true;
        for (int i = 0; i < particles.Length; i++)
        {
            m = particles[i].main;
            m.loop = false;
        }
        step = 0;
    }

    public void ReturnToZero()
    {
        //score
        unit = -1.0f * score / 7f;
        lastScore = score;
        score = 0;
        //
        combo = 0;
        maxCombo = 0;
        perfect = 0;
        intact = 0;
        defective = 0;
        miss = 0;
        lastIntegrity = integrity;
        if (increment)
            integrity = 0;
        else
            integrity = 1;
        integrityUnit = (integrity - lastIntegrity) / 7;
        comboState = -1;
       //
        step = 0;
        movement = true;
        reduction = true;
        comboFade = false;
    }

    public static void launchEffect(Vector2 pos, int judge,bool mute)
    {
        //effect
        Instantiate(judgeEffect, playground).SendMessage("SetEffect", new EffectData(pos, judge, mute));
        unit = 0;
        if (judge != 0)
        {
            //score
            switch (judge)
            {
                case 3:
                    perfect++;
                    unit = 1.0f * scoreUnit / 7;
                    break;
                case 2:
                    intact++;
                    unit = 1.0f * scoreUnit / 7;
                    break;
                case 4:
                case 1:
                    defective++;
                    unit = 0.75f * scoreUnit / 7;
                    break;
            }
            lastScore = score;
            score = (perfect + intact) * scoreUnit + defective * scoreUnit * 0.75f;
            //combo
            combo++;
            comboState = 1;
            sizeState = 1;
            if (combo > maxCombo)
            {
                maxCombo = combo;
                //fc
                if (maxCombo == Launcher.totalNote)
                {
                    comboFade = true;
                    comboState = 0;
                    FC.SetActive(true);
                    Launcher.fcWait = 2.4f;
                }
            }
        }
        else
        {
            combo = 0;
            comboState = -1;
            miss++;
        }
        //integrity
        lastIntegrity = integrity;
        if (increment)
            integrity = 1.0f * (perfect + intact * 0.8f + defective * 0.5f) / Launcher.totalNote;
        else
            integrity = 1 - 1.0f * (miss + intact * 0.2f + defective * 0.5f) / Launcher.totalNote;
        integrityUnit = (integrity - lastIntegrity) / 7;
        //
        step = 0;
        movement = true;
    }
}