using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorNote : MonoBehaviour
{
    public GameObject effect;
    private static int startSide = -1;
    private Vector2 dirct;
    private Vector2 totalDist;
    private float resTime;
    private float pasTime;
    private CanvasRenderer cr;
    private float fadeRate = 0;

    void Start()
    {
        transform.localPosition = new Vector2(570, 0) * startSide;
        startSide = -startSide;
        cr = GetComponent<CanvasRenderer>();
        totalDist = new Vector2(570, 0) * startSide;
        resTime = Vector3.Magnitude(totalDist) / Launcher.noteSpeed;
        dirct = new Vector2(startSide, 0);
        cr.SetAlpha(fadeRate);
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeRate < 1)
        {
            fadeRate += 0.05f;
            cr.SetAlpha(fadeRate);
        }
        pasTime += Time.deltaTime;
        transform.localPosition = Vector2.zero - totalDist * (resTime - pasTime) / resTime;
        if (pasTime >= resTime)
        {
            Instantiate(effect, transform.parent);
            Destroy(gameObject);
        }
    }

}
