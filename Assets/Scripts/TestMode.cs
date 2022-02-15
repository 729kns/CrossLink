using System.Collections;
using UnityEngine;

public class TestMode : MonoBehaviour
{
    public bool test;
    public bool autoMode;
    public bool demo;
    public static bool auto;
    public string song;
    public int level;
    public float speed;
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_EDITOR
        if(!demo)
            auto = autoMode;
#endif
        if (!test)
        {
            song = InfoPanel.songName;
            level = InfoPanel.level;
            speed = Launcher.noteSpeed;
        }
        else
        {
            Application.targetFrameRate = 60;
            SwitchSceneAnimation.atListScene = false;
            InfoPanel.songName = song;
            InfoPanel.level = level;
            Launcher.noteSpeed = speed;
            InfoPanel.songInfo = "undefined";
            InfoPanel.illustration = "undefined";
            InfoPanel.noter = "undefined";
        }
    }
#if UNITY_EDITOR
    void Update()
    {
        if(!demo && auto != autoMode)
            auto = autoMode;
    }
#endif
}
