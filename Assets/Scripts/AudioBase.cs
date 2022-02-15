using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AudioBase : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
	private int MusicID=-1;
	private int FileID=-1;
	private	int SoundID;
	private static int audioCount=0;
    private static bool poolMaked;
#else
    private AudioSource unityAudio;
#endif
    public string audioName;
    public bool isMusic;
    private bool loaded;
    private bool playAfterLoad;
    private bool loop;
    private bool setAfterLoad;
    private float setValue;

#if UNITY_ANDROID && !UNITY_EDITOR
	public int geta(){
		if (isMusic)
			return MusicID;
		else
			return FileID;
	}
#endif

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
		if (!poolMaked) {
			AndroidNativeAudio.makePool(20);
			poolMaked = true;
		}
#else
        gameObject.AddComponent<AudioSource>().playOnAwake = false;
        unityAudio = GetComponent<AudioSource>();
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
	void OnApplicationQuit(){
		if (poolMaked) {
			for (int i = 1; i < audioCount + 1; i++)
				AndroidNativeAudio.unload (i);
			AndroidNativeAudio.releasePool ();
			poolMaked = false;
		}
	}
#endif

    void OnEnable()
    {
        if (audioName != null && audioName != "" && !loaded)
            LoadAudio(audioName);
    }

    public void LoadAudio(string name)
    {
        loaded = false;
#if UNITY_ANDROID && !UNITY_EDITOR
            if (isMusic)
            {
             //   Debug.Log(name + "---------------");
                if (MusicID != -1)
                    release();
                MusicID = ANAMusic.load("Songs/" + name, false, true, isLoaded);
            }
            else
            {
                if (FileID != -1)
                    release();
                FileID = AndroidNativeAudio.load("Sounds/" + name);
                loaded = true;
                audioCount++;
            }
#else
        if (isMusic)
            StartCoroutine(loadFile("file://" + Application.streamingAssetsPath + "/Songs/" + name));
        else
            StartCoroutine(loadFile("file://" + Application.streamingAssetsPath + "/Sounds/" + name));
#endif
    }

    public void LoadTestAudio()
    {
        loaded = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        if (MusicID != -1)
            release();
        MusicID = ANAMusic.load("Beat.ogg", false, true, isLoaded);
#else
        StartCoroutine(loadFile("file://" + Application.streamingAssetsPath + "/Beat.ogg"));
#endif
    }

    public void Play()
    {
        if (!loaded)
        {
            playAfterLoad = true;
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
		if (isMusic)
			ANAMusic.play (MusicID, null);
		else
			SoundID = AndroidNativeAudio.play (FileID);
#else
        unityAudio.Play();
#endif
    }

    public void PlayLoop()
    {
        if (!loaded)
        {
            playAfterLoad = true;
            loop = true;
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!isMusic)
            SoundID = AndroidNativeAudio.play(FileID, 1, -1, 1, -1, 1);
        else
        {
            ANAMusic.play(MusicID, null);
            ANAMusic.setLooping(MusicID, true);
        }
#else
        unityAudio.loop = true;
        unityAudio.Play();
#endif
    }

    public void Stop()
    {
        if (!loaded)
            return;
#if UNITY_ANDROID && !UNITY_EDITOR
		if (isMusic) {
			ANAMusic.pause (MusicID);
			ANAMusic.seekTo (MusicID, 0);
		} else {
			AndroidNativeAudio.stop (SoundID);
		}
#else
        unityAudio.Stop();
#endif
    }

    public void Pause()
    {
        if (!loaded)
            return;
#if UNITY_ANDROID && !UNITY_EDITOR
		if (isMusic)
			ANAMusic.pause (MusicID);
		else
			AndroidNativeAudio.pause (SoundID);
#else
        unityAudio.Pause();
#endif
    }
    public void SetVolume(float value)
    {
        if (!loaded)
        {
            setAfterLoad = true;
            setValue = value;
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
		if(isMusic)
			ANAMusic.setVolume (MusicID, value);
		else
			AndroidNativeAudio.setVolume(SoundID, value);
#else
        unityAudio.volume = value;
#endif
    }
    public void SetPlayTime(float value)
    {
        if (!loaded || !isMusic)
            return;
#if UNITY_ANDROID && !UNITY_EDITOR
		ANAMusic.seekTo (MusicID, (int)(value * 1000));
#else
        unityAudio.time = value;
#endif
    }
    public float GetDuration()
    {
        if (!loaded || !isMusic)
            return 0;
#if UNITY_ANDROID && !UNITY_EDITOR
		return ANAMusic.getDuration (MusicID)*0.001f;
#else
        return unityAudio.clip.length;
#endif
    }

    public float GetPlayTime()
    {
        if (!loaded || !isMusic)
            return 0;
#if UNITY_ANDROID && !UNITY_EDITOR
		return ANAMusic.getCurrentPosition (MusicID) / 1000f;
#else
        return unityAudio.time;
#endif
    }

    public bool isPlaying()
    {
        if (!loaded || !isMusic)
            return false;
#if UNITY_ANDROID && !UNITY_EDITOR
		return ANAMusic.isPlaying (MusicID);
#else
        return unityAudio.isPlaying;
#endif
    }

    public void release()
    {
        loaded = false;
#if UNITY_ANDROID && !UNITY_EDITOR
		if (isMusic) {
			if (MusicID != -1){
				if (ANAMusic.isPlaying (MusicID))
					ANAMusic.pause (MusicID);
				ANAMusic.release (MusicID);
				MusicID = -1;
			}
		}
		else{
			if (FileID != -1) {
				AndroidNativeAudio.unload (FileID);
				audioCount--;
				FileID = -1;
			}
		}
#else
        unityAudio.clip = null;
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
	void isLoaded(int id){
		loaded = true;
		if (playAfterLoad) {
			Play ();
			playAfterLoad = false;
		}
		if (setAfterLoad) {
			SetVolume (setValue);
			setAfterLoad = false;
		}
	}
#endif

    public bool HasLoaded()
    {
        return loaded;
    }

    #if !UNITY_ANDROID || UNITY_EDITOR
    IEnumerator loadFile(string path)
    {
        UnityWebRequest www;
        if (isMusic)
        {
            www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS);
            ((DownloadHandlerAudioClip)www.downloadHandler).compressed = false;
            ((DownloadHandlerAudioClip)www.downloadHandler).streamAudio = true;
        }
        else
            www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
        
        yield return www.SendWebRequest();

        unityAudio.clip = DownloadHandlerAudioClip.GetContent(www);
        loaded = true;
        www.Dispose();
        if (playAfterLoad)
        {
            if (loop)
                PlayLoop();
            else
                Play();
            playAfterLoad = false;
        }
        if (setAfterLoad)
        {
            SetVolume(setValue);
            setAfterLoad = false;
        }
    }
    #endif
}