using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    private RectTransform rt;
    private CanvasRenderer cr;
    private float fadeRate = 1;
    private Vector3 scaleSpeed = new Vector3(0.12f, 0.12f, 0);
    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        cr = GetComponent<CanvasRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Launcher.pause)
            return;
        rt.localScale += scaleSpeed;
        fadeRate -= 0.08f;
        cr.SetAlpha(fadeRate);
        if (fadeRate <= 0)
            Destroy(gameObject);
    }
}
