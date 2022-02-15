using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneAnimation : MonoBehaviour
{
    public static bool switching;
    public GameObject block;
    public RectTransform line, loading;
    public static bool atListScene = true, showUp;
    public static int nextScene;
 //   public static float screenRatio;
    private Transform[] blocks;
    private int step;
    private int lineCount;
    private int countX, countY;
    private int skipFrames;
    private float currentScale;
    // Start is called before the first frame update

    void Awake()
    {
        GetComponent<RectTransform>().sizeDelta = CameraAdjust.screenSize;
        countX = (int)(CameraAdjust.screenSize.x / 120 + 0.5f);
        countY = (int)(CameraAdjust.screenSize.y / 120 + 0.5f);
        blocks = new Transform[countX * countY];
        int count = 0;
        Vector2 sib = new Vector2(60 + CameraAdjust.screenSize.x * 0.5f, 60 + CameraAdjust.screenSize.y * 0.5f);
        for (int i = 1; i <= countX; i++)
        {
            for (int j = 1; j <= countY; j++)
            {
                blocks[count] = Instantiate(block, transform).transform;
                blocks[count].localPosition = new Vector2(i * 120, j * 120) - sib;
                count++;
            }
        }
        line.SetAsLastSibling();
        loading.SetAsLastSibling();
        for (int i = 0; i < blocks.Length - 1; i++)
        {
            int r = Random.Range(i + 1, blocks.Length - 1);
            Transform temp = blocks[i];
            blocks[i] = blocks[r];
            blocks[r] = temp;
        }
        line.sizeDelta = new Vector2(CameraAdjust.screenSize.x, 10);
        if (SceneManager.GetActiveScene().buildIndex == 1 && atListScene)
            gameObject.SetActive(false);
    }

    void OnEnable()
    {
        switching = true;
        step = 0;
        lineCount = 0;
        if (!atListScene)
        {
            for (int i = 0; i < blocks.Length; i++)
                blocks[i].localScale = Vector2.one;
            line.localScale = Vector2.one;
            loading.localScale = Vector2.one;
            skipFrames = 2;
            currentScale = 1;
        }
        else
        {
            for (int i = 0; i < blocks.Length; i++)
                blocks[i].localScale = Vector2.zero;
            line.localScale = new Vector2(0, 1);
            loading.localScale = new Vector2(0, 1);
            currentScale = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!atListScene)
        {
            if (skipFrames > 0)
            {
                skipFrames--;
                return;
            }
            switch (step)
            {
                case 0:
                    currentScale -= 0.05f;
                    line.localScale = new Vector2(currentScale, 1);
                    loading.localScale = new Vector2(1, currentScale);
                    if (currentScale <= 0.025f)
                    {
                        currentScale = 1;
                        step++;
                    }
                    break;
                case 1:
                    currentScale -= 0.2f;
                    for (int i = 0; i < countY; i++)
                        blocks[i * countX + lineCount].localScale = new Vector2(currentScale, currentScale);
                    if (currentScale <= 0.05f)
                    {
                        lineCount++;
                        currentScale = 1;
                        if (lineCount >= countX)
                        {
                            switching = false;
                            gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }
        else
        {
            switch (step)
            {
                case 0:
                    currentScale += 0.2f;
                    for (int i = 0; i < countY; i++)
                        blocks[i * countX + lineCount].localScale = new Vector2(currentScale, currentScale);
                    if (currentScale >= 0.95f)
                    {
                        lineCount++;
                        currentScale = 0;
                        if (lineCount >= countX)
                            step++;
                    }
                    break;
                case 1:
                    currentScale += 0.05f;
                    line.localScale = new Vector2(currentScale, 1);
                    loading.localScale = new Vector2(1, currentScale);
                    if (currentScale >= 0.975f)
                    {
                        step++;
                        switching = false;
                        //switch scene
                        atListScene = false;
                        SceneManager.LoadScene(nextScene);
                    }
                    break;
                
            }
        }
    }
}
