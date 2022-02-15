using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongList : MonoBehaviour
{
    public GameObject track;
    public GameObject switchScene;
    public FloatingCoverGenerater fcg;
    public Text sortBy;
    public RectTransform content;
    public RectTransform arrow;
    public NodeNet nodeNet;
    public AudioBase shutter;
    public AudioBase click;
    private bool skipClick;
    private RectTransform[] tracks;
    private Entry[] entryScripts;
    private int currentList;
    private int currentSong;
    private string lastSongName;
    private CanvasRenderer[] crs;
    private float fadeRate;
    private int state;
    private int operate;

    private string[] songNames;
    private string[] songInfos;
    private int[] songRanks;

    private int[] listCount;
    private int first;
    private int viewCount;
    private bool alphaSort;

    string tempName;
    bool firstTime = true;
    bool selFirst;
    // Start is called before the first frame update
    void Awake()
    {
        viewCount = (int)(CameraAdjust.screenSize.y / 160 + 0.5f) + 2;
        listCount = new int[viewCount];
        for (int i = 0; i < listCount.Length; i++)
            listCount[i] = i;
        currentList = PlayerPrefs.GetInt(PackList.packCount + "LastList", 2);
        lastSongName = PlayerPrefs.GetString(PackList.packCount + "LastSong", null);
        operate = currentList;
        if (alphaSort)
            sortBy.text = "Alphabet";
        else
            sortBy.text = "Difficulty";
        
        ReadList((TextAsset)Resources.Load("Song Lists/" + PackList.packCount + "_" + currentList));
        firstTime = false;
        if (!SwitchSceneAnimation.atListScene)
        {
            CreateList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case -1:
                if (fadeRate > 0)
                {
                    fadeRate -= 0.05f;
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                    for (int i = 0; i < tracks.Length; i++)
                        tracks[i].localPosition += new Vector3(0, 5, 0);
                }
                else
                {
                    arrow.gameObject.SetActive(false);
                    arrow.SetParent(content);
                    for (int i = 0; i < tracks.Length; i++)
                        DestroyImmediate(tracks[i].gameObject);
                    switch (operate)
                    {
                        case 0:
                        case 1:
                        case 2:
                            currentList = operate;
                            ReadList((TextAsset)Resources.Load("Song Lists/" + PackList.packCount + "_" + currentList));
                            CreateList();
                            break;
                        case 5:
                            state = 0;
                            break;
                    }
                }
                break;
            case 0:
                break;
            case 1:
                if (fadeRate < 1)
                {
                    fadeRate += 0.05f;
                    for (int i = 0; i < crs.Length; i++)
                        crs[i].SetAlpha(fadeRate);
                    for (int i = 0; i < tracks.Length; i++)
                        tracks[i].localPosition -= new Vector3(0, 5, 0);
                }
                else
                {
                    state = 0;
                }
                break;
        }
    }

    public void SwitchList(int opt) //0=LO,1=LI,2=EX,4=SORT,5=CLOSE
    {
        if (operate == opt && opt != 4)
            return;
        if (opt == 4)
        {
            alphaSort = !alphaSort;
            if (alphaSort)
                sortBy.text = "Alphabet";
            else
                sortBy.text = "Difficulty";
        }
        else
            operate = opt;
        state = -1;
        shutter.Play();
        skipClick = true;
    }

    public void OnSliding()
    {
        //列表向下
        while (first + viewCount < songNames.Length && content.anchoredPosition.y + tracks[listCount[0]].anchoredPosition.y > 80)
        {
            if (entryScripts[listCount[0]].GetTrackNum() == currentSong)
            {
                arrow.gameObject.SetActive(false);
                entryScripts[listCount[0]].SlideSelect(false);
            }
            else if (first + viewCount == currentSong && !selFirst)
            {
                arrow.gameObject.SetActive(true);
                arrow.SetParent(tracks[currentSong % viewCount]);
                arrow.localPosition = new Vector2(660, 0);
                entryScripts[listCount[0]].SlideSelect(true);
            }
            tracks[listCount[0]].anchoredPosition = new Vector2(tracks[listCount[0]].anchoredPosition.x, tracks[listCount[viewCount - 1]].anchoredPosition.y - 160);
            entryScripts[listCount[0]].ChangeInfo(first + viewCount, currentList, songNames[first + viewCount], songInfos[first + viewCount], songRanks[first + viewCount]);
            
            first++;
            int temp = listCount[0];
            for (int i = 0; i < viewCount - 1; i++)
                listCount[i] = listCount[i + 1];
            listCount[viewCount - 1] = temp;
        }
        //列表向上
        while (first > 0 && content.anchoredPosition.y + tracks[listCount[viewCount - 1]].anchoredPosition.y < -CameraAdjust.screenSize.y - 80)
        {
            if (entryScripts[listCount[viewCount - 1]].GetTrackNum() == currentSong)
            {
                arrow.gameObject.SetActive(false);
                entryScripts[listCount[viewCount - 1]].SlideSelect(false);
            }
            else if (first - 1 == currentSong && !selFirst)
            {
                arrow.gameObject.SetActive(true);
                arrow.SetParent(tracks[currentSong % viewCount]);
                arrow.localPosition = new Vector2(660, 0);
                entryScripts[listCount[viewCount - 1]].SlideSelect(true);
            }
            tracks[listCount[viewCount - 1]].anchoredPosition = new Vector2(tracks[listCount[viewCount - 1]].anchoredPosition.x, tracks[listCount[0]].anchoredPosition.y + 160);
            entryScripts[listCount[viewCount - 1]].ChangeInfo(first - 1, currentList, songNames[first - 1], songInfos[first - 1], songRanks[first - 1]);
            
            first--;
            int temp = listCount[viewCount - 1];
            for (int i = viewCount - 1; i > 0; i--)
                listCount[i] = listCount[i - 1];
            listCount[0] = temp;
        }
        if (selFirst)
        {
            entryScripts[currentSong % viewCount].Select();
            selFirst = false;
        }
    }

    public void CreateList()
    {
        selFirst = true;
        content.sizeDelta = new Vector2(content.sizeDelta.x, songNames.Length * 160);
        if (songNames.Length < viewCount - 2 || currentSong < viewCount - 2)
        {
            content.anchoredPosition = new Vector2(0, 0);
            first = 0;
        }
        else
        {
            content.anchoredPosition = new Vector2(0, 160 + 160 * currentSong - CameraAdjust.screenSize.y);
            first = currentSong - viewCount;
        }
        for (int i = 0; i < listCount.Length; i++)
            listCount[i] = (first + i) % viewCount;
        tracks = new RectTransform[viewCount > songNames.Length ? songNames.Length : viewCount];
        entryScripts = new Entry[tracks.Length];

        for (int i = 0; i < tracks.Length; i++)
        {
            int t = listCount[i];
            tracks[t] = Instantiate(track, content).GetComponent<RectTransform>();
            entryScripts[t] = tracks[t].GetComponent<Entry>();
            tracks[t].localPosition = new Vector2(90, 20 - 160 * (first + i));   //20=-80+100
            entryScripts[t].ChangeInfo(first + i, currentList, songNames[first + i], songInfos[first + i], songRanks[first + i]);
        }
        crs = content.GetComponentsInChildren<CanvasRenderer>();
        fadeRate = 0;
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(fadeRate);
        state = 1;
        OnSliding();
    }

    public void CloseList()
    {
        transform.parent.SendMessage("StopMusic");
        operate = 5;
        state = -1;
    }

    void Switch2Play()
    {
        PlayerPrefs.SetInt(PackList.packCount + "LastList", currentList);
        //  Debug.Log(songNames[currentSong]);
        PlayerPrefs.SetString(PackList.packCount + "LastSong", songNames[currentSong]);
        click.Play();
        transform.parent.SendMessage("StopMusic");
        SwitchSceneAnimation.nextScene = 2;
        SwitchSceneAnimation.atListScene = true;
        switchScene.SetActive(true);
    }

    void SelectTrack(int num)
    {
        if ((currentSong - num) % viewCount != 0 && !selFirst)
            entryScripts[currentSong % viewCount].Unselect();
        currentSong = num;
        transform.parent.SendMessage("SwitchPanel", new Info(songNames[currentSong], currentList));
        arrow.gameObject.SetActive(true);
        arrow.SetParent(tracks[num % viewCount]);
        arrow.localPosition = new Vector2(660, 0);
        if (!selFirst)
            nodeNet.ChangeNet();
        if (!skipClick)
            shutter.Play();
        else
            skipClick = false;
    }

    void ReadList(TextAsset list)
    {
        string data = list.text;
        System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
        string[] lines = data.Split(new char[] { '\n' }, option);
        char[] spliter = new char[1] { ',' };

        if (alphaSort)
            lines = lines.OrderBy(p => p).ToArray();
        else
            lines = lines.OrderBy(a => int.Parse(System.Text.RegularExpressions.Regex.Match(a.Trim(), @"\d+$").Value)).ToArray();

        if (!firstTime)
        {
            tempName = songNames[currentSong];
        }
        else if (lastSongName != null)
        {
            tempName = lastSongName;
        }
        currentSong = 0;

        songNames = new string[lines.Length];
        songInfos = new string[lines.Length];
        songRanks = new int[lines.Length];
        
        for (int i = 0; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(spliter, option);
            songNames[i] = datas[0];
            songInfos[i] = datas[1];
            songRanks[i] = int.Parse(datas[2]);
            if(tempName.Equals(songNames[i]))
                currentSong = i;
        }
        fcg.GetList(songNames);
    }
}