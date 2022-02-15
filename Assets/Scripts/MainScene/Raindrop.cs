using System.Collections;
using UnityEngine;

public class Raindrop : MonoBehaviour
{
    private ParticleSystem particle;
    private RectTransform rt;
    private float destructCount = 1.2f;
    // Use this for initialization
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(Random.Range(CameraAdjust.screenSize.x * -0.5f, CameraAdjust.screenSize.x * 0.5f), CameraAdjust.screenSize.y * 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!DropEmitter.stop && rt.anchoredPosition.y > -CameraAdjust.screenSize.y * 0.55f)
            transform.Translate(new Vector3(0, -0.04f, 0));
        else
        {
            if (particle.isPlaying)
                particle.Stop();
            destructCount -= Time.deltaTime;
            if (destructCount <= 0)
                Destroy(gameObject);
        }
    }
}
