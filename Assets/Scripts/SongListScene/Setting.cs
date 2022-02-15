using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    public bool integrityIncrement;
    public bool soundOn;
    public RectTransform caster;
    public Sprite setL, setR;
    public Image sfButton, isButton, atButton;
    public Text sound, integrity, speed, delay, auto;
    private CanvasRenderer[] crs;
    private Button[] buttons;
    private float fadeRate;
    private int state;
    private RectTransform rt;
    private float scale;
    // Start is called before the first frame update
    void Awake()
    {
        caster.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        soundOn = PlayerPrefs.GetInt("Sound Effect", 1) == 1 ? true : false;
        if (soundOn)
        {
            sfButton.sprite = setR;
            sound.text = "ON";
        }
        else
        {
            sfButton.sprite = setL;
            sound.text = "OFF";
        }
        integrityIncrement = PlayerPrefs.GetInt("Integrity Setting", 1) == 1 ? true : false;
        if (integrityIncrement)
        {
            isButton.sprite = setR;
            integrity.text = "Increment";
        }
        else
        {
            isButton.sprite = setL;
            integrity.text = "Decrement";
        }
        //----auto
        atButton.sprite = setL;
        auto.text = "OFF";
        TestMode.auto = false;
        //-----
        Launcher.noteSpeed = PlayerPrefs.GetFloat("Note Speed", 800);
        speed.text = Launcher.noteSpeed.ToString();
        Launcher.audioDelay = PlayerPrefs.GetFloat("Audio Delay", 0);
        delay.text = string.Format("+{0} ms", Launcher.audioDelay.ToString());
        //
        rt = GetComponent<RectTransform>();
        crs = GetComponentsInChildren<CanvasRenderer>();
        buttons = GetComponentsInChildren<Button>();
        gameObject.SetActive(false);
        
    }

    private void OnEnable()
    {
        fadeRate = 0;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
        scale = 0.72f;
        rt.localScale = new Vector2(scale, scale);
        rt.localPosition = new Vector2(-140, -140);
        state = 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case -1:
                if (scale > 0.73f)
                {
                    scale -= 0.02f;
                    fadeRate -= 0.08f;
                    rt.localPosition = new Vector2(rt.localPosition.x - 10, rt.localPosition.y - 10);
                    rt.localScale = new Vector2(scale, scale);
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                }
                else
                    gameObject.SetActive(false);
                break;
            case 0:
                break;
            case 1:
                if (scale < 0.99)
                {
                    scale += 0.02f;
                    fadeRate += 0.08f;
                    rt.localPosition = new Vector2(rt.localPosition.x + 10, rt.localPosition.y + 10);
                    rt.localScale = new Vector2(scale, scale);
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                }
                else
                {
                 //   rt.localPosition = Vector2.zero;
                 //   rt.localScale = Vector2.one;
                    state = 0;
                    for (int i = 0; i < buttons.Length; i++)
                        buttons[i].enabled = true;
                }
                break;
        }
    }

    public void Confirm()
    {
        PlayerPrefs.SetInt("Sound Effect", soundOn ? 1 : 0);
        PlayerPrefs.SetInt("Integrity Setting", integrityIncrement ? 1 : 0);
        PlayerPrefs.SetFloat("Note Speed", Launcher.noteSpeed);
        PlayerPrefs.SetFloat("Audio Delay", Launcher.audioDelay);
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].enabled = false;
        state = -1;
    }

    public void SetSound()
    {
        soundOn = !soundOn;
        if (soundOn)
        {
            sfButton.sprite = setR;
            sound.text = "ON";
        }
        else
        {
            sfButton.sprite = setL;
            sound.text = "OFF";
        }
    }

    public void SetIntegrity()
    {
        integrityIncrement = !integrityIncrement;
        if (integrityIncrement)
        {
            isButton.sprite = setR;
            integrity.text = "Increment";
        }
        else
        {
            isButton.sprite = setL;
            integrity.text = "Decrement";
        }
    }

    public void SetAuto()
    {
        TestMode.auto = !TestMode.auto;
        if (TestMode.auto)
        {
            atButton.sprite = setR;
            auto.text = "ON";
        }
        else
        {
            atButton.sprite = setL;
            auto.text = "OFF";
        }
    }

    //2:++ /1:+ /-1:- /-2:--
    //max 1500 min 300
    public void SetSpeed(int opt)
    {
        float preSpeed = Launcher.noteSpeed;
        switch (opt)
        {
            case -1:
                Launcher.noteSpeed -= 10;
                break;
            case -2:
                Launcher.noteSpeed -= 100;
                break;
            case 1:
                Launcher.noteSpeed += 10;
                break;
            case 2:
                Launcher.noteSpeed += 100;
                break;
        }
        if (Launcher.noteSpeed > 1500)
            Launcher.noteSpeed = 1500;
        else if (Launcher.noteSpeed < 300)
            Launcher.noteSpeed = 300;
        speed.text = Launcher.noteSpeed.ToString();
    //    Indicator.offset += 570 / preSpeed - 570 / Launcher.noteSpeed;
    }

    //2:++ /1:+ /-1:- /-2:--
    public void SetDelay(int opt)
    {
        float preDelay = Launcher.audioDelay;
        switch (opt)
        {
            case -1:
                Launcher.audioDelay -= 1;
                break;
            case -2:
                Launcher.audioDelay -= 10;
                break;
            case 1:
                Launcher.audioDelay += 1;
                break;
            case 2:
                Launcher.audioDelay += 10;
                break;
        }
        if (Launcher.audioDelay > 1200)
            Launcher.audioDelay = 1200;
        else if (Launcher.audioDelay < -1200)
            Launcher.audioDelay = -1200;
        if (Launcher.audioDelay >= 0)
            delay.text = string.Format("+{0} ms", Launcher.audioDelay.ToString());
        else
            delay.text = string.Format("{0} ms", Launcher.audioDelay.ToString());
     //   Indicator.offset += (Launcher.audioDelay - preDelay) * 0.001f;
    }
}
