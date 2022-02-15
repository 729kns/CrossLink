using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    public GameObject note;
    public AudioBase beat;
    public Image simulateButton;
    public Sprite simulateStart, simulateStop;
    public GameObject caster;
    //   private float timeCount;
    private float delayTime;
    private float waitTime, audioWaitTime;
    private float playTime, moveTime;
    private int count;
    private bool initialized;
    private bool simulating;

    void OnEnable()
    {
        simulating = true;
        SwitchSimulate();
    }

    public void Close()
    {
        if(simulating)
            SwitchSimulate();
    }

    // Update is called once per frame
    void Update()
    {
        if (!simulating)
            return;
        if (!initialized)
        {
            waitTime += Time.deltaTime;
            if (waitTime >= delayTime && beat.HasLoaded() && !beat.isPlaying())
            {
                beat.Play();
                audioWaitTime = waitTime;
            }
            if (waitTime >= delayTime - Launcher.audioDelay * 0.001f && beat.isPlaying())
                initialized = true;
        }
        if (beat.isPlaying())
            playTime = beat.GetPlayTime() + audioWaitTime;
        else
            playTime += Time.deltaTime;

        if (count < 32 && playTime >= count)
        {
            count++;
            Instantiate(note, transform);
        }

        if (count >= 32 && playTime >= beat.GetDuration() + audioWaitTime)
        {
            SwitchSimulate();
        }
    }

    public void SwitchSimulate()
    {
        if (!simulating)
        {
            initialized = false;
            waitTime = 0;
            playTime = 0;
            count = 0;
            moveTime = 570f / Launcher.noteSpeed;
            delayTime = moveTime + Launcher.audioDelay * 0.001f;
            simulating = true;
            simulateButton.sprite = simulateStop;
            caster.SetActive(true);
            beat.LoadTestAudio();
            transform.parent.parent.SendMessage("SwitchVolume", true);
        }
        else
        {
            for (int i = 4; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
            beat.Stop();
            simulating = false;
            simulateButton.sprite = simulateStart;
            caster.SetActive(false);
            transform.parent.parent.SendMessage("SwitchVolume", false);
        }
    }
}
