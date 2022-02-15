using System.Collections;
using UnityEngine;

public class FloatingCover : MonoBehaviour
{
    private static Vector2 lastPos;
    private CanvasRenderer[] crs;
    private int state = 1;
    private float stayTime;
    private float fadeRate;
    // Start is called before the first frame update
    void Start()
    {
        crs = GetComponentsInChildren<CanvasRenderer>();
        while (true)
        {
            Vector2 iniPos = new Vector2(Random.Range(-CameraAdjust.screenSize.x * 0.5f, CameraAdjust.screenSize.x * 0.5f), Random.Range(-CameraAdjust.screenSize.y * 0.5f, CameraAdjust.screenSize.y * 0.5f));
            if (Vector2.Distance(iniPos, lastPos) > 300)
            {
                lastPos = iniPos;
                iniPos *= transform.parent.localScale;
                transform.position = iniPos;
                break;
            }
        }
        float scale = Random.Range(0.27f, 0.62f);
        transform.localScale = new Vector2(scale, scale);
        stayTime = Random.Range(4, 6);
        for (int i = 0; i < crs.Length; i++)
            crs[i].SetAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (FloatingCoverGenerater.close)
            state = -1;
        transform.Translate(new Vector2(0, 0.1f) * Time.deltaTime);
        switch (state)
        {
            case -1:
                fadeRate -= 0.02f;
                for (int i = 0; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                if (fadeRate <= 0)
                    Destroy(gameObject);
                break;
            case 0:
                stayTime -= Time.deltaTime;
                if (stayTime <= 0)
                    state = -1;
                break;
            case 1:
                fadeRate += 0.02f;
                for (int i = 0; i < crs.Length; i++)
                    crs[i].SetAlpha(fadeRate);
                if (fadeRate >= 1)
                    state = 0;
                break;
        }
    }
}
