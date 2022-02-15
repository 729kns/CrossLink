using System.Collections;
using UnityEngine;

public class MainSceneParticle : MonoBehaviour
{
    private ParticleSystem particle;
    public Material digitRain;
    // Start is called before the first frame update
    void Awake()
    {
        float lifeTime = CameraAdjust.screenSize.y / 154;
        float span = CameraAdjust.screenSize.x / 215;
        particle = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule m = particle.main;
        m.startLifetime = lifeTime;
        ParticleSystem.ShapeModule s = particle.shape;
        s.radius = span;
        digitRain.SetTextureScale("_MainTex", Vector2.one);
        digitRain.SetColor("_EmisColor", new Color(0, 0.65f, 0.65f));

        ResumeParticles();
    }

    public void PauseParticles()
    {
        particle.Pause();
    }

    public void ResumeParticles()
    {
        particle.Play();
    }
}
