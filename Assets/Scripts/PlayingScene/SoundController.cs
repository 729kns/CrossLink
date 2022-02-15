using System.Collections;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private static AudioBase[] hits;
    private static int hitCount, endCount, len;
    // Start is called before the first frame update
    void Start()
    {
        hits = GetComponentsInChildren<AudioBase>();
        len = hits.Length / 2;
        hitCount = 1;
        endCount = len + 1;
    }

    public static void SoundHit()
    {
        hits[hitCount].Play();
        hitCount = (hitCount % len) + 1;
    }

    public static void SoundEnd()
    {
        hits[endCount].Play();
        endCount = (endCount % len) + len + 1;
    }
}
