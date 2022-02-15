using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AudioVisualize : MonoBehaviour
{
    public AudioSource music;
    public Subtitles subtitles;
    public TextAsset songs;
    public Transform grid;
    public GameObject bar;
    public GameObject block;
    public static int xEdge, yEdge;
    public static Vector2 sib;
    private RectTransform[] rts = new RectTransform[64];
    private SpectrumBlock[] blocks;
    private RectTransform rt;
    private string[] songNames;
    private float[] scaleData, decreaseRate, bandData, highestBuffer;
    private float[] spectrumData = new float[512];
    private float scaleRate;
    private float amplified;
    private int preSong = -1;
    private int blockCount;
    private float preBeatStrength;
    private int state = -2, step;
    private bool loading;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        scaleData = new float[rts.Length];
        decreaseRate = new float[rts.Length];
        highestBuffer = new float[rts.Length];
        for (int i = 0; i < highestBuffer.Length; i++)
            highestBuffer[i] = 0.04f;
        bandData = new float[rts.Length];

        scaleRate = CameraAdjust.screenSize.y * 0.0024f;
        float width = CameraAdjust.screenSize.x / rts.Length;
        rts[0] = bar.GetComponent<RectTransform>();
        rts[0].anchoredPosition = new Vector2(0.5f * width, 0);
        rts[0].sizeDelta = new Vector2(CameraAdjust.screenSize.x / 128, 128);
        for (int i = 1; i < rts.Length; i++)
        {
            rts[i] = Instantiate(bar, transform).GetComponent<RectTransform>();
            rts[i].anchoredPosition = new Vector2((0.5f + i) * width, 0);
        }

        string data = songs.text;
        System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
        songNames = data.Split(new char[] { '\n' }, option);
        for (int i = 0; i < songNames.Length; i++)
            songNames[i] = songNames[i].Trim();

        xEdge = (int)(CameraAdjust.screenSize.x / 120 + 0.5f) + 1;
        yEdge = (int)(CameraAdjust.screenSize.y / 120 + 0.5f) + 1;
        sib = new Vector2(60 + CameraAdjust.screenSize.x * 0.5f, 60 + CameraAdjust.screenSize.y * 0.5f);
        blocks = new SpectrumBlock[xEdge * yEdge / 5 + 1];
        blocks[0] = block.GetComponent<SpectrumBlock>();
        for (int i = 1; i < blocks.Length; i++)
        {
            blocks[i] = Instantiate(block, grid).GetComponent<SpectrumBlock>();
        }
        rt.anchoredPosition -= new Vector2(0, 50);
    }

    // Update is called once per frame
    void Update()
    {
        SpectrumBarVisualize();
        switch (state)
        {
            case 1:
                if (step < 50)
                {
                    step++;
                    rt.anchoredPosition += new Vector2(0, 1);
                }
                else
                    state = 0;
                break;
            case 0:
                BlockVisualize();
                if (!music.isPlaying && !loading)
                    SwitchSong();
                break;
            case -1:
                if (music.volume > 0)
                {
                    music.volume -= 0.02f;
                    rt.anchoredPosition -= new Vector2(0, 1);
                }
                break;
        }
    }

    void SwitchSong()
    {
        int t = preSong;
        while (t == preSong)
            t = Random.Range(0, songNames.Length);
        preSong = t;

#if UNITY_ANDROID && !UNITY_EDITOR
		StartCoroutine (loadFile ("jar:file://" + Application.dataPath + "!/assets/Songs/" + songNames[preSong] + ".ogg"));
#else
        StartCoroutine(loadFile(Application.streamingAssetsPath + "/Songs/" + songNames[preSong] + ".ogg"));
#endif
        subtitles.Reset(songNames[preSong]);
    }

    public void CloseVisualizer()
    {
        state = -1;
    }

    public void ActiveVisualizer()
    {
        step = 0;
        state = 1;
    }

    void SpectrumBarVisualize()
    {
        amplified = 0;
        if (music.isPlaying && state != -1)
        {
            music.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);
            int sampleCount = 1;
            int power = 0;
            int count = 0;

            for (int i = 0; i < bandData.Length; i++)
            {
                float avg = 0;
                if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
                {
                    power++;
                    sampleCount = (int)Mathf.Pow(2, power);
                    if (power == 3)
                        sampleCount -= 2;
                }
                for (int j = 0; j < sampleCount; j++)
                {
                    avg += spectrumData[count];
                    amplified += spectrumData[count];
                    count++;
                }
                bandData[i] = avg / sampleCount;

                //clamp
                if (bandData[i] > highestBuffer[i])
                    highestBuffer[i] = bandData[i];
                bandData[i] /= highestBuffer[i];
            }
        }

        //decrease
        for (int i = 0; i < scaleData.Length; i++)
        {
            if (bandData[i] > scaleData[i] && music.isPlaying && state != -1)
            {
                scaleData[i] = bandData[i];
                decreaseRate[i] = 0.005f;
            }
            else
            {
                scaleData[i] -= decreaseRate[i];
                if (scaleData[i] < 0)
                    scaleData[i] = 0;
                decreaseRate[i] *= 1.2f;
            }
            float scale = (int)((scaleData[i] * scaleRate + 0.05f + amplified * 0.1f) * 4 + 0.5f) * 0.25f - 0.01f;
            if (scale <= 0)
                scale = 0.24f;
            rts[i].localScale = new Vector2(1, scale);
        }
    }

    void BlockVisualize()
    {
        float beatStrength = 0;
        for (int i = 0; i < 6; i++)
            beatStrength += bandData[i];
        if (beatStrength > preBeatStrength)
        {
            preBeatStrength = beatStrength;
            //   beatStrength -= amplified * 0.1f;
            int actNum = (int)(beatStrength * 20 - 6) / 5;
            for (int i = 0; i < actNum; i++)
            {
                blocks[blockCount].Activate();
                blockCount = (blockCount + 1) % blocks.Length;
            }
        }
        else
            preBeatStrength -= 0.02f;
    }

    IEnumerator loadFile(string path)
    {
        loading = true;
        UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS);
        ((DownloadHandlerAudioClip)www.downloadHandler).compressed = false;
        ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
        yield return www.SendWebRequest();
        music.clip = DownloadHandlerAudioClip.GetContent(www);
        loading = false;
        music.Play();
        www.Dispose();
    }
}
