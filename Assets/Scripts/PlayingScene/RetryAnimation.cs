using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RetryAnimation : MonoBehaviour
{
    public RectTransform leftLine, rightLine;
    public RectTransform NoteMask;
    public GameObject countDown;
    private float moveDistance;
    private Vector2 leftInitPos, rightInitPos;
    private float passTime;
    
    // Start is called before the first frame update
    void Awake()
    {
        leftInitPos = new Vector2(-Screen.currentResolution.width / 2 - 64, 0);
        rightInitPos = new Vector2(Screen.currentResolution.width / 2 + 64, 0);
    }

    private void OnEnable()
    {
        leftLine.localPosition = leftInitPos;
        rightLine.localPosition = rightInitPos;
        NoteMask.GetComponent<Mask>().enabled = true;
        NoteMask.sizeDelta = CameraAdjust.screenSize;
        passTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        passTime += Time.deltaTime;
        moveDistance = 1.0f * (Screen.currentResolution.width + 128) * passTime / 1.8f;
        leftLine.localPosition = new Vector2(leftInitPos.x + moveDistance, 0);
        rightLine.localPosition = new Vector2(rightInitPos.x - moveDistance, 0);
        NoteMask.sizeDelta = new Vector2(rightLine.localPosition.x - leftLine.localPosition.x - 128, NoteMask.sizeDelta.y);
        if (passTime >= 1.8f)
        {
            for (int i = 0; i < NoteMask.childCount; i++)
                Destroy(NoteMask.GetChild(i).gameObject);
            NoteMask.GetComponent<Mask>().enabled = false;
            NoteMask.sizeDelta = CameraAdjust.screenSize;
            ScoreManager.reduction = false;
            countDown.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
