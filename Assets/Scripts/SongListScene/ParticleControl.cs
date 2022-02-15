using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControl : MonoBehaviour
{
    public ParticleSystem[] particles;
    private float[] originValue;
    private float[] offset, timeCount;
    private ParticleSystem.ForceOverLifetimeModule[] fo;
    // Start is called before the first frame update
    void Start()
    {
        originValue = new float[particles.Length];
        offset = new float[particles.Length];
        timeCount = new float[particles.Length];
        fo = new ParticleSystem.ForceOverLifetimeModule[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            originValue[i] = particles[i].forceOverLifetime.yMultiplier;
            fo[i] = particles[i].forceOverLifetime;
            offset[i] = Random.Range(-0.0015f, 0.0015f);
            timeCount[i] = Random.Range(3f, 5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < particles.Length; i++)
        {
            timeCount[i] -= Time.deltaTime;
            fo[i].yMultiplier += offset[i];
            if (timeCount[i] <= 0)
            {
                if (fo[i].yMultiplier > originValue[i])
                    offset[i] = Random.Range(-0.0015f, 0);
                else
                    offset[i] = Random.Range(0, 0.0015f);
                timeCount[i] = Random.Range(3f, 5f);
            }
        }
    }
}
