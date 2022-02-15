using System.Collections;
using UnityEngine;

public class CameraAdjust : MonoBehaviour
{
    //  public Vector2 screenSize;
    public static Vector2 screenSize;
    private Camera cam;
    private float ratio;

    void Awake()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        cam = GetComponent<Camera>();
        ratio = 1.0f * Screen.height / Screen.width;
        if (ratio >= 0.5625f)   //16:9--4:3
        {
            screenSize = new Vector2(1920, 1920 * ratio);
        }
        else if (ratio >= 0.5f) //--18:9
        {
            screenSize = new Vector2(1080 / ratio, 1080);
        }
        else   //>18:9
        {
            cam.rect = new Rect(0.5f - ratio, cam.rect.y, 2 * ratio, cam.rect.height);
            screenSize = new Vector2(2160, 1080);
        }
    }
}
