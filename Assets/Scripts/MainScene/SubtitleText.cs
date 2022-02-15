using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleText : MonoBehaviour {
	private string subtitle;
	private Text t;
    private int state;

	// Use this for initialization
	void Awake () {
		t = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.frameCount % 3 == 0)
        {
            switch (state)
            {
                case 1:
                    if (t.text.Length < subtitle.Length)
                        t.text = subtitle.Substring(0, t.text.Length + 1);
                    else
                        state = 0;
                    break;
                case -1:
                    if (t.text.Length > 0)
                        t.text = subtitle.Substring(0, t.text.Length - 1);
                    else
                    {
                        subtitle = null;
                        state = 0;
                    }
                    break;
            }
        }
	}

	public void Reset(){
		if (subtitle == null)
			return;
        state = -1;
	}

	public void SetText(string str){
		subtitle = str;
		t.text = "";
        state = 1;
	}

	public void SetTextImmediately(string str){
		subtitle = str;
		t.text = subtitle;
        state = 0;
	}
}
