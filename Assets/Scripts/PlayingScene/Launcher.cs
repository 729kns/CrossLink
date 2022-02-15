using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviour
{
    public AudioBase music;
    public static float noteSpeed;
    public static float audioDelay;
    public static Vector2 initialPos;
    public static int totalNote;
    public static bool pause;
    public RectTransform progress;
    public GameObject[] notes;
    public GameObject pausePanel;
    public RectTransform playground;
    //
    private int noteCount;
    private Note[] noteDatas;
    private float waitTime, audioWaitTime;
    private float delayTime;
    private float playTime;
    private bool initialized;
    public static float fcWait;

    // Start is called before the first frame update
    void Awake()
    {
        initialPos = new Vector2(-CameraAdjust.screenSize.x * 0.5f - 90, -CameraAdjust.screenSize.y * 0.5f - 90);
        //   playground.GetComponent<CanvasRenderer>().SetAlpha(1);
    }

    private void OnEnable()
    {
        audioDelay = audioDelay * 0.001f;
        fcWait = 0;
        pause = true;
        delayTime = CameraAdjust.screenSize.x / noteSpeed + audioDelay;
        readNoteData((TextAsset)Resources.Load("Scores/" + InfoPanel.songName + "_" + InfoPanel.level));
        music.LoadAudio(InfoPanel.songName + ".ogg");
    }

    private void OnDisable()
    {
        audioDelay = audioDelay * 1000;
        pause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (ScoreManager.reduction && progress.sizeDelta.x > 0)
            progress.sizeDelta -= new Vector2(CameraAdjust.screenSize.x * 0.01f, 0);
        if (pause || SwitchSceneAnimation.switching)
            return;
        if (!initialized)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= delayTime && music.HasLoaded() && !music.isPlaying())
            {
                music.Play();
                audioWaitTime = waitTime;
            }
            if (waitTime >= delayTime - audioDelay && music.isPlaying())
            {
                initialized = true;
            }
        }
        if (music.isPlaying())
            playTime = music.GetPlayTime() + audioWaitTime;
        else
            playTime += Time.deltaTime;
        if (noteCount >= noteDatas.Length && playTime >= music.GetDuration() + audioWaitTime + fcWait)//music.GetDuration() + delayTime
        {
            transform.SendMessage("Settle");
            pause = true;
            music.Stop();
            music.release();
        }

        while (noteCount < noteDatas.Length && playTime >= noteDatas[noteCount].launchTime)
        {
            noteDatas[noteCount].id = noteCount;
            Instantiate(notes[noteDatas[noteCount].type], playground).SendMessage("Initialize", noteDatas[noteCount]);
            noteCount++;
        }
        progress.sizeDelta = new Vector2(CameraAdjust.screenSize.x * (playTime / (music.GetDuration() + audioWaitTime)), 0);
    }

    public void Begin()
    {
        pause = false;
        if (initialized)
            music.Play();
    }

    public void Retry()
    {
        waitTime = 0;
        fcWait = 0;
        playTime = 0;
        music.SetPlayTime(0);
        noteCount = 0;
        initialized = false;
    }

    public void Pause()
    {
        pause = true;
        pausePanel.SetActive(true);
        music.Pause();
    }

    void readNoteData(TextAsset asset)
    {
        string txtSongData = asset.text;
        string[] lines = txtSongData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        char[] spliter = new char[1] { ',' };
        noteCount = lines.Length;
        noteDatas = new Note[noteCount];
        totalNote = noteCount;

        for (int i = 0; i < noteCount; i++)
        {
            string[] data = lines[i].Split(spliter);
            noteDatas[i] = new Note();
            noteDatas[i].type = int.Parse(data[0]) - 1;
            switch (int.Parse(data[2]))
            {
                case 1:
                    noteDatas[i].dirct = Vector2.left;
                    break;
                case 2:
                    noteDatas[i].dirct = Vector2.right;
                    break;
                case 3:
                    noteDatas[i].dirct = Vector2.up;
                    break;
                case 4:
                    noteDatas[i].dirct = Vector2.down;
                    break;
            }
            noteDatas[i].startPos = new Vector2((float.Parse(data[3]) - 0.5f) * CameraAdjust.screenSize.x, (float.Parse(data[4]) - 0.5f) * CameraAdjust.screenSize.y);
            noteDatas[i].distance = noteDatas[i].dirct * (-initialPos + noteDatas[i].startPos * noteDatas[i].dirct);
            noteDatas[i].moveTime = Vector3.Magnitude(noteDatas[i].distance) / noteSpeed;
            noteDatas[i].launchTime = float.Parse(data[1]);
            noteDatas[i].waitTime = delayTime - noteDatas[i].moveTime - audioDelay;
            //    noteDatas[i].time = float.Parse(data[1]) - noteDatas[i].moveTime + delayTime;
            if (data[data.Length - 1] == "d")
                noteDatas[i].dual = true;
            switch (noteDatas[i].type)
            {
                case 1:
                    noteDatas[i].length = float.Parse(data[5]);
                    totalNote++;
                    break;
                case 2:
                    noteDatas[i].length = float.Parse(data[7]);
                    noteDatas[i].endPos = new Vector2((float.Parse(data[5]) - 0.5f) * CameraAdjust.screenSize.x, (float.Parse(data[6]) - 0.5f) * CameraAdjust.screenSize.y);
                    totalNote++;
                    break;
            }
        }
        //    Array.Sort(noteDatas);
        noteCount = 0;
    }
}

class Note : IComparable
{
    public int id;
    public int type;
    public float launchTime, waitTime;
    public Vector2 distance;
    public float moveTime;
    public Vector2 dirct;
    public Vector2 startPos, endPos;
    public float length;
    public bool dual;

    public Note()
    {
        id = 0;
        type = 0;
        launchTime = 0;
        waitTime = 0;
        distance = Vector2.zero;
        moveTime = 0;
        dirct = Vector2.zero;
        startPos = Vector2.zero;
        endPos = Vector2.zero;
        length = 0;
        dual = false;
    }

    public int CompareTo(object obj)
    {
        Note o = obj as Note;
        return launchTime.CompareTo(o.launchTime);
    }
}
