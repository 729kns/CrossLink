using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Image illust, rank;
    public Sprite[] ranks;
    public Text score, integrity, info;
    public RectTransform[] panels;
    public RectTransform panelRoot;
    public GameObject fullCombo;

    public static string songName;
    public static string songInfo;
    public static string illustration;
    public static string noter;
    public static int level;

    private int state;
    private AudioBase music;
    private int audioState, volumeState;
    private float volume, vp = 0.9f;
    private float startPoint, loopTime;
    private float scale, fadeRate;
    private CanvasRenderer[] crs;
    private bool close, skipOnce, keepMusic;
    // Start is called before the first frame update
    void Awake()
    {
        music = GetComponent<AudioBase>();
        scale = 0.74f;
        for (int i = 0; i < panels.Length; i++)
            panels[i].localScale = new Vector2(scale, scale);
        crs = panelRoot.GetComponentsInChildren<CanvasRenderer>();
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
        if (SwitchSceneAnimation.atListScene)
            skipOnce = true;
        songName = null;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case -1:
                if (scale > 0.75f)
                {
                    scale -= 0.02f;
                    fadeRate -= 0.08f;
                    panelRoot.localPosition = new Vector2(panelRoot.localPosition.x + 10, panelRoot.localPosition.y - 10);
                    for (int i = 0; i < panels.Length; i++)
                        panels[i].localScale = new Vector2(scale, scale);
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                }
                else
                {
                    if (!close)
                    {
                        ChangeInfo();
                        panelRoot.anchoredPosition = new Vector2(-260, 140);
                        state = 1;
                    }
                    else
                        state = 0;
                }
                break;
            case 0:
            case 2:
                break;
            case 1:
                if (scale < 0.99f)
                {
                    scale += 0.02f;
                    fadeRate += 0.08f;
                    panelRoot.localPosition = new Vector2(panelRoot.localPosition.x - 10, panelRoot.localPosition.y - 10);
                    for (int i = 0; i < panels.Length; i++)
                        panels[i].localScale = new Vector2(scale, scale);
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                }
                else
                {
                    state = 2;
                }
                break;
        }

        switch (audioState)
        {
            case -2:
                if (volume > 0)
                {
                    volume -= 0.02f;
                    music.SetVolume(volume * vp);
                }
                else
                    music.Stop();
                break;
            case -1:
                volume -= 0.01f;
                music.SetVolume(volume * vp);
                if (volume <= 0)
                {
                    audioState = 2;
                    loopTime = 16;
                    music.SetPlayTime(startPoint);
                }
                break;
            case 0:
                loopTime -= Time.deltaTime;
                if (loopTime <= 0)
                    audioState = -1;
                break;
            case 1:
                if (music.HasLoaded() && !music.isPlaying())
                {
                    music.SetPlayTime(startPoint);
                    loopTime = 16;
                    volume = 0;
                    music.SetVolume(0);
                    music.Play();
                    audioState = 2;
                }
                break;
            case 2:
                volume += 0.01f;
                music.SetVolume(volume * vp);
                if (volume >= 1)
                    audioState = 0;
                break;
        }
        if (music.isPlaying())
            switch (volumeState)
            {
                case -1:
                    if (vp > 0.2f)
                    {
                        vp -= 0.02f;
                        music.SetVolume(volume * vp);
                    }
                    else
                        volumeState = 0;
                    break;
                case 0:
                    break;
                case 1:
                    if (vp < 0.9)
                    {
                        vp += 0.02f;
                        music.SetVolume(volume * vp);
                    }
                    else
                        volumeState = 0;
                    break;
            }
    }

    void StopMusic()
    {
        if (music.isPlaying())
            audioState = -2;
    }

    void SwitchVolume(bool lower)
    {
        if (lower)
            volumeState = -1;
        else
            volumeState = 1;
    }

    void SwitchPanel(Info info)
    {
        keepMusic = false;
        if (songName != info.name)
            StopMusic();
        else if(!skipOnce)
            keepMusic = true;
        songName = info.name;
        level = info.level;
        if (state == 2)
        {
            scale = 1;
            fadeRate = 1;
            state = -1;
            if (!keepMusic)
                audioState = 1;
        }
        else
        {
            if (skipOnce)
            {
                skipOnce = false;
                return;
            }
            InitializePanel();
        }
    }

    public void InitializePanel()
    {
        ChangeInfo();
        scale = 0.74f;
        fadeRate = 0;
        panelRoot.anchoredPosition = new Vector2(-260, 140);
        state = 1;
        if(!keepMusic)
            audioState = 1;
    }

    public void ClosePanel()
    {
        StopMusic();
        close = true;
        state = -1;
    }

    private void ChangeInfo()
    {
        if (PlayerPrefs.GetInt(songName + level.ToString() + "fc", 0) == 1)
            fullCombo.SetActive(true);
        else
            fullCombo.SetActive(false);
        illust.sprite = Resources.Load<Sprite>("Song Cover/" + songName);
        if (!keepMusic)
            music.LoadAudio(songName + ".ogg");
        int r = PlayerPrefs.GetInt(songName + level.ToString() + "rank", 0);
        if (r == 0)
        {
            panels[0].localPosition = new Vector2(panels[0].localPosition.x, 110);
            panels[1].gameObject.SetActive(false);
            panels[2].gameObject.SetActive(false);
            panels[3].localPosition = new Vector2(panels[3].localPosition.x, -350);
        }
        else
        {
            panels[0].localPosition = new Vector2(panels[0].localPosition.x, 170);
            panels[1].gameObject.SetActive(true);
            panels[2].gameObject.SetActive(true);
            panels[3].localPosition = new Vector2(panels[3].localPosition.x, -420);
            rank.sprite = ranks[r - 1];
            int s = PlayerPrefs.GetInt(songName + level.ToString() + "score", 0);
            score.text = s.ToString("D7");
            if (s == 1000000)
                score.color = new Color(1, 0.75f, 0);
            else
                score.color = Color.white;
            float i = PlayerPrefs.GetFloat(songName + level.ToString() + "integrity", 0);
            if (i == 1)
                integrity.color = new Color(1, 0.75f, 0);
            else
                integrity.color = new Color(0, 0.78f, 1);
            integrity.text = (i * 100).ToString("F2") + "%";
        }
        TextAsset ta = (TextAsset)Resources.Load("Song Info/" + songName);
        string[] infos = ta.text.Split(new char[] { ',' });
        songInfo = infos[0];
        illustration = infos[1];
        noter = infos[2];
        info.text = songInfo + "\n" + illustration + "\n" + noter;
        startPoint = float.Parse(infos[3]);
    }
}
