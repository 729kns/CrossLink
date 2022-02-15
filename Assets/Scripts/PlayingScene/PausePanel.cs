using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    public CanvasRenderer combo;
    public Launcher laucher;
    public BackgroundAnimation backAni;
    public GameObject countDown;
    public GameObject retry;
    public Button[] buttons;
    public GameObject switchScene;
    public RectTransform rt;
    private float fadeRate;
    private CanvasRenderer[] crs;
    private int state;
    private int command;    //1=back 2=retry 3=exit
    private float comboFadeRate, currentComboFadeRate;
    private Vector2 sib;
    

    private void Awake()
    {
     //   rt = GetComponent<RectTransform>();
        crs = GetComponentsInChildren<CanvasRenderer>();

    }
    // Start is called before the first frame update
    void OnEnable()
    {
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].enabled = false;
        fadeRate = 0;
        command = 0;
        for (int i = 2; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
        rt.sizeDelta = new Vector2(240, 300);
        //    caster.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        sib = new Vector2((CameraAdjust.screenSize.x - 240) / 16, 0);
        state = 1;
        comboFadeRate = combo.GetAlpha();
        currentComboFadeRate = comboFadeRate;
        backAni.PauseParticles();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 1:
                fadeRate += 0.0625f;
                for (int i = 2; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                if (currentComboFadeRate > 0)
                {
                    currentComboFadeRate -= 0.1f;
                    combo.SetAlpha(currentComboFadeRate);
                }
                else
                    combo.SetAlpha(0);
                rt.sizeDelta += sib;
                if (fadeRate >= 1)
                {
                    for (int i = 0; i < buttons.Length; i++)
                        buttons[i].enabled = true;
                    state = 0;
                }
                break;
            case 0:
                break;
            case -1:
                fadeRate -= 0.0625f;
                for (int i = 2; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                if (currentComboFadeRate < comboFadeRate)
                {
                    currentComboFadeRate += 0.1f;
                    combo.SetAlpha(currentComboFadeRate);
                }
                else
                    combo.SetAlpha(comboFadeRate);
                rt.sizeDelta -= sib;
                if (fadeRate <= 0)
                {
                    switch (command)
                    {
                        case 1:
                            countDown.SetActive(true);
                            break;
                        case 2:
                            retry.SetActive(true);
                            laucher.Retry();
                            break;
                        case 3:
                            SwitchSceneAnimation.atListScene = true;
                            SwitchSceneAnimation.nextScene = 1;
                            switchScene.SetActive(true);
                            break;
                    }
                    backAni.ResumeParticles();
                    gameObject.SetActive(false);
                }
                break;
        }
    }

    //1=back 2=retry 3=exit
    public void Operation(int pauseCommand)
    {
        state = -1;
        command = pauseCommand;
        for (int i = 0; i < buttons.Length; i++)
            buttons[i].enabled = false;
    }
}
