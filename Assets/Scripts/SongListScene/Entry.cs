using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Entry : MonoBehaviour
{
    public Text title, musician, rank;
    public Image levelImage;
    public Image clearState;
    public Sprite[] levels, states;
    private RectTransform rt;
    private string songName;
    private int trackNum;
    private int level;
    private int state;
    // Start is called before the first frame update
    void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case -1:
                if (rt.sizeDelta.x > 60)
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x - 4, 20);
                else
                {
                    rt.sizeDelta = new Vector2(60, 20);
                    state = 0;
                }
                rt.localPosition = new Vector2(60 + 0.5f * rt.sizeDelta.x, rt.localPosition.y);
                break;
            case 0:
            case 2:
                break;
            case 1:
                if (rt.sizeDelta.x < 140)
                    rt.sizeDelta = new Vector2(rt.sizeDelta.x + 4, 20);
                else
                {
                    rt.sizeDelta = new Vector2(140, 20);
                    state = 2;
                }
                rt.localPosition = new Vector2(60 + 0.5f * rt.sizeDelta.x, rt.localPosition.y);
                break;
        }
    }

    public void Select()
    {
        if (state == 2)
        {
         //   Debug.Log("Start");
            transform.parent.parent.parent.SendMessage("Switch2Play", trackNum);
            return;
        }
        if (state <= 0)
        {
            state = 1;
            transform.parent.parent.parent.SendMessage("SelectTrack", trackNum);
        }
    }

    public void Unselect()
    {
        state = -1;
    }

    public int GetTrackNum()
    {
        return trackNum;
    }

    public void SlideSelect(bool sel)
    {
        if (sel)
        {
            rt.sizeDelta = new Vector2(140, 20);
            state = 2;
        }
        else
        {
            rt.sizeDelta = new Vector2(60, 20);
            state = 0;
        }
        rt.localPosition = new Vector2(60 + 0.5f * rt.sizeDelta.x, rt.localPosition.y);
    }

    public void ChangeInfo(int setNum, int setLevel, string setTitle, string setMusician, int setRank)
    {
        levelImage.sprite = levels[setLevel];
        trackNum = setNum;
        songName = setTitle;
        level = setLevel;
        title.text = songName;
        musician.text = setMusician;
        rank.text = setRank.ToString();
        //read data
        int tagState = PlayerPrefs.GetInt(songName + level.ToString() + "rank", 0);
        if (tagState == 0 || tagState == 1)
            clearState.gameObject.SetActive(false);
        else
        {
            clearState.gameObject.SetActive(true);
            if (tagState == 7)
                clearState.sprite = states[2];
            else if (PlayerPrefs.GetInt(songName + level.ToString() + "fc", 0) == 1)
                clearState.sprite = states[1];
            else
                clearState.sprite = states[0];
        }
    }
}

struct Info
{
    public string name;
    public int level;
    public Info(string setName,int setLevel)
    {
        name = setName;
        level = setLevel;
    }
}
