using System.Collections;
using UnityEngine;

public class Subtitles : MonoBehaviour {
	public AudioSource music;
	private SubtitleText[] lines;
	private float[] subtitleTimeLine;
	private string[] subtitles;
	private int row,lineCount;
    private bool closing;
	// Use this for initialization
	void Awake () {
		lines = GetComponentsInChildren<SubtitleText> ();
	}

	// Update is called once per frame
	void Update () {
        if (closing)
            return;
		if (music.isPlaying && lineCount < subtitleTimeLine.Length && music.time >= subtitleTimeLine [lineCount]) {
			if (row >= lines.Length) {
                for (int i = 1; i < lines.Length - 1; i++)
                    lines[i].SetTextImmediately(subtitles[lineCount - row + i + 1]);
				lines [lines.Length - 1].SetText (subtitles [lineCount]);
			} else {
				lines [row].SetText (subtitles [lineCount]);
				row++;
			}
			lineCount++;
		}
	}

	public void Reset(string songName){
        for (int i = 1; i < lines.Length; i++)
            lines[i].Reset();
		row = 1;
        lines[0].SetText(songName);
		lineCount = 0;
		readSubtitleData (Resources.Load ("Subtitles/" + songName) as TextAsset);
	}

	public void Close(){
        closing = true;
        subtitleTimeLine = null;
		subtitles = null;
		for (int i = 0; i < lines.Length; i++)
			lines [i].Reset ();
	}

	void readSubtitleData(TextAsset asset){
		string txtSongData=asset.text;
		System.StringSplitOptions option=System.StringSplitOptions.RemoveEmptyEntries;
		string[] lines=txtSongData.Split(new char[]{'\n'},option);
		char[] spliter =new char[1]{','};
		subtitles = new string[lines.Length];
		subtitleTimeLine = new float[lines.Length];
		for (int i = 0; i < lines.Length; i++) {
			string[] data=lines[i].Split(spliter,option);
			subtitleTimeLine [i] = float.Parse (data [0]);
			subtitles [i] = data [1];
		}
	}
}
