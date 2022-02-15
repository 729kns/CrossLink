using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PauseButton : MonoBehaviour
{
    public Sprite pauseButton, pauseConfirm;
    public Launcher laucher;
    private Image pauseButtonImage;
    private float countTime;
    private bool confirming;

    // Start is called before the first frame update
    void Start()
    {
        pauseButtonImage = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (confirming)
        {
            countTime -= Time.deltaTime;
            if (countTime <= 0)
            {
                confirming = false;
                pauseButtonImage.sprite = pauseButton;
            }
        }
    }

    public void Pause()
    {
        if (Launcher.pause)
            return;
        if (!confirming)
        {
            pauseButtonImage.sprite = pauseConfirm;
            countTime = 1.5f;
            confirming = true;
        }
        else
        {
            confirming = false;
            laucher.Pause();
            pauseButtonImage.sprite = pauseButton;
        }
    }
}
