using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SongInfo : MonoBehaviour
{
    public Image cover;
    public Text title, level, info;
    public Launcher laucher;
    private int step;
    private float timeCount;
    private RectTransform rt;
    private CanvasRenderer[] crs;
    private float fadeRate;
    // Start is called before the first frame update
    void Start()
    {
        crs = GetComponentsInChildren<CanvasRenderer>();
        rt = GetComponent<RectTransform>();
        title.text = InfoPanel.songName;
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
        info.text = string.Format("{0}\n{1}\n{2}", InfoPanel.songInfo, InfoPanel.illustration, InfoPanel.noter);
        cover.sprite = Resources.Load<Sprite>("Song Cover/" + InfoPanel.songName);
        rt.localScale = Vector2.zero;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (SwitchSceneAnimation.switching)
            return;
        switch (step)
        {
            case 0:
                fadeRate += 0.05f;
                rt.localScale = new Vector2(fadeRate, fadeRate);
                for (int i = 0; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                if (fadeRate >= 1)
                    step++;
                break;
            case 1:
                timeCount += Time.deltaTime;
                if (timeCount >= 2)
                    step++;
                break;
            case 2:
                fadeRate -= 0.05f;
                for (int i = 0; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                if (fadeRate <= 0)
                {
                    laucher.Begin();
                    Destroy(gameObject);
                }
                break;
        }
    }
}
