using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundAnimation : MonoBehaviour
{
    public ParticleSystem[] particles;
    public Material digitRain;
    // Start is called before the first frame update
    void Awake()
    {
        Image im = GetComponent<Image>();
        im.sprite = Resources.Load<Sprite>("Song Background/Blur/" + InfoPanel.songName);
        im.rectTransform.sizeDelta = CameraAdjust.screenSize;
        float lifeTime = 1.0f * CameraAdjust.screenSize.x / 160;
        ParticleSystem.MainModule m;
        for (int i = 0; i < particles.Length; i++)
        {
            m = particles[i].main;
            m.startLifetime = lifeTime;
            particles[i].Simulate(0, false, true);
            particles[i].Play();
        }
        digitRain.SetTextureScale("_MainTex", Vector2.one);
        digitRain.SetColor("_EmisColor", Color.white);
    }

    public void PauseParticles()
    {
        for(int i = 0; i < particles.Length; i++)
        {
            particles[i].Pause();
        }
    }

    public void ResumeParticles()
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
        }
    }
}
