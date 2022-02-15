using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettlePanel : MonoBehaviour
{
    public Text title;
    public Text level;
    public Text HO, compare;
    public Slider integritySlider;
    public Image cover;
    public Image rank;
    public Text score, percentage, combo, details;
    public RectTransform confirmButton;
    public Sprite[] ranks;
    public GameObject switchScene;
    private RectTransform rt;
    private CanvasRenderer[] rankCR;
    private CanvasRenderer[] crs;
    private CanvasRenderer buttonCR;
    private float integrity;
    private float scoreNum;
    private float scoreUnit, inteUnit;
    private int settleState;
    private float itemFadeRate;
    private float rankScale;
    private int state;
    private int rankID;
    private float fadeRate;
    // Start is called before the first frame update
    void Start()
    {
        confirmButton.GetComponent<Button>().enabled = false;
        crs = GetComponentsInChildren<CanvasRenderer>();
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(0);
        rt = GetComponent<RectTransform>();
        rankCR = rank.GetComponentsInChildren<CanvasRenderer>();
        buttonCR = confirmButton.GetComponent<CanvasRenderer>();
        rt.sizeDelta = new Vector2(CameraAdjust.screenSize.x, CameraAdjust.screenSize.y);
        //title
        title.text = InfoPanel.songName + "\n" + InfoPanel.songInfo;
        switch (InfoPanel.level)
        {
            case 0:
                level.text = "Logarithmic";
                level.color = new Color(0, 1, 0.75f);
                break;
            case 1:
                level.text = "Linear";
                level.color = new Color(0, 0.75f, 1);
                break;
            case 2:
                level.text = "Exponential";
                level.color = new Color(1, 0.5f, 1);
                break;
        }
        //integrity
        integrity = ScoreManager.integrity;
        inteUnit = integrity / 100f;
        //score
        scoreUnit = 1.0f * ScoreManager.score / 100;
        int diff = (int)ScoreManager.score - PlayerPrefs.GetInt(InfoPanel.songName + InfoPanel.level + "score");
        compare.canvasRenderer.SetAlpha(0);
        compare.text = (diff >= 0 ? "+" : "-") + diff.ToString();
        //details
        combo.text = ScoreManager.maxCombo.ToString();
        if (ScoreManager.maxCombo == Launcher.totalNote)
            combo.color = new Color(1, 0.75f, 0);
        details.text = ScoreManager.perfect + "\n" + ScoreManager.intact + "\n" + ScoreManager.defective + "\n" + ScoreManager.miss;
        //rank
        if (integrity == 1)
            rankID = 6;
        else
        {
            if ((int)(ScoreManager.score + 0.5f) == 1000000)
                rankID = 5;
            else if (ScoreManager.score >= 980000)
                rankID = 4;
            else if (ScoreManager.score >= 950000)
                rankID = 3;
            else if (ScoreManager.score >= 900000)
                rankID = 2;
            else if (ScoreManager.score >= 800000)
                rankID = 1;
        }
        rank.sprite = ranks[rankID];
        rankScale = 1.4f;
        rank.rectTransform.localScale = new Vector2(rankScale, rankScale);
        //cover
        cover.sprite = Resources.Load<Sprite>("Song Cover/" + InfoPanel.songName);
        //save
        if (PlayerPrefs.GetFloat(InfoPanel.songName + InfoPanel.level.ToString() + "integrity", 0) < ScoreManager.integrity)
            PlayerPrefs.SetFloat(InfoPanel.songName + InfoPanel.level.ToString() + "integrity", ScoreManager.integrity);
        if (PlayerPrefs.GetInt(InfoPanel.songName + InfoPanel.level.ToString() + "score", 0) < (int)(ScoreManager.score + 0.5f))
            PlayerPrefs.SetInt(InfoPanel.songName + InfoPanel.level.ToString() + "score", (int)(ScoreManager.score + 0.5f));
        int prevRank = PlayerPrefs.GetInt(InfoPanel.songName + InfoPanel.level.ToString() + "rank", 0);
        if (prevRank <= 1 && rankID >= 1)
            PlayerPrefs.SetInt(PackList.packCount + "_" + InfoPanel.level.ToString() + "_c", PlayerPrefs.GetInt(PackList.packCount + "_" + InfoPanel.level.ToString() + "_c", 0) + 1);
        if (prevRank < rankID + 1)
            PlayerPrefs.SetInt(InfoPanel.songName + InfoPanel.level.ToString() + "rank", rankID + 1);
        //save check full combo
        if (PlayerPrefs.GetInt(InfoPanel.songName + InfoPanel.level.ToString() + "fc", 0) != 1 && ScoreManager.maxCombo == Launcher.totalNote)
        {
            PlayerPrefs.SetInt(InfoPanel.songName + InfoPanel.level.ToString() + "fc", 1);
            PlayerPrefs.SetInt(PackList.packCount + "_" + InfoPanel.level.ToString() + "_f", PlayerPrefs.GetInt(PackList.packCount + "_" + InfoPanel.level.ToString() + "_f", 0) + 1);
        }
        state = 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case 1:
                rt.localPosition -= new Vector3(0, 1, 0);
                fadeRate += 0.02f;
                for (int i = 0; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                for (int i = 0; i < rankCR.Length; i++)
                    rankCR[i].SetAlpha(0);
                buttonCR.SetAlpha(0);
                if (fadeRate >= 1)
                    state = 0;
                break;
            case 0:
                //integrity
                if (integritySlider.value < integrity)
                    integritySlider.value += inteUnit;
                else
                {
                    integritySlider.value = ScoreManager.integrity;
                    if (integritySlider.value >= 1)
                        percentage.color = new Color(1, 0.75f, 0);
                }
                percentage.text = (integritySlider.value * 100).ToString("F2") + "%";
                //score
                switch (settleState)
                {
                    case 0:
                        scoreNum += scoreUnit;
                        if (scoreNum >= ScoreManager.score) {
                            scoreNum = ScoreManager.score;
                            settleState = 1;
                        }
                        int num = (int)(scoreNum + 0.5f);
                        if (num == 1000000)
                            score.color = new Color(1, 0.75f, 0);
                        score.text = num.ToString("D7");
                        HO.text = System.Convert.ToString(num, 16) + "\n" + System.Convert.ToString(num, 8);
                        break;
                    case 1:
                        compare.rectTransform.localPosition += new Vector3(0, 2, 0);
                        itemFadeRate += 0.04f;
                        compare.canvasRenderer.SetAlpha(itemFadeRate);
                        if (itemFadeRate >= 1)
                        {
                            itemFadeRate = 0;
                            settleState = 2;
                        }
                        break;
                    case 2:
                        itemFadeRate += 0.05f;
                        for (int i = 0; i < rankCR.Length; i++)
                            rankCR[i].SetAlpha(itemFadeRate);
                        rankScale -= 0.02f;
                        rank.rectTransform.localScale = new Vector2(rankScale, rankScale);
                        if (itemFadeRate >= 1)
                        {
                            itemFadeRate = 0;
                            settleState = 3;
                        }
                        break;
                    case 3:
                        itemFadeRate += 0.04f;
                        confirmButton.localPosition += new Vector3(0, 1, 0);
                        buttonCR.SetAlpha(itemFadeRate);
                        if (itemFadeRate >= 1)
                        {
                            confirmButton.GetComponent<Button>().enabled = true;
                            settleState = -1;
                        }
                        break;
                }
                break;
        }
    }

    public void Confirm()
    {
        SwitchSceneAnimation.atListScene = true;
        confirmButton.GetComponent<Button>().enabled = false;
        SwitchSceneAnimation.nextScene = 1;
        switchScene.SetActive(true);
    }
}
