using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PackList : MonoBehaviour
{
    public TextAsset list;
    public Title title;
    public RecordPanel recordPanel;
    public GameObject packCover;
    public RectTransform content;
    public static int packCount;
    private PackCover[] covers;
    private ScrollRect sr;
    private string[] packNames;
    private int[] totalSongs;
    private Vector2 tarPos;
    private bool recal, moving;
    private float inertiaTime;
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<ScrollRect>();
        ReadList(list);
        title.GetTitle(packNames[packCount]);
        recordPanel.GetRecord(totalSongs[packCount]);
        CreatList();
        for (int i = 0; i < covers.Length; i++)
            covers[i].SetAlpha(-content.anchoredPosition.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (recal && !Input.GetMouseButton(0))
        {
            inertiaTime += Time.deltaTime;
            if (inertiaTime >= 0.2f)
            {
                if (!moving)
                {
                    packCount = (int)(-content.anchoredPosition.x / 1280f + 0.5f);
                    tarPos = new Vector2(packCount * -1280, 0);
                    title.GetTitle(packNames[packCount]);
                    recordPanel.GetRecord(totalSongs[packCount]);
                    moving = true;
                }
                else
                {
                    if (content.anchoredPosition.x < tarPos.x && content.anchoredPosition.x + 100 <= tarPos.x)
                        content.anchoredPosition += new Vector2(100, 0);
                    else if (content.anchoredPosition.x > tarPos.x && content.anchoredPosition.x - 100 >= tarPos.x)
                        content.anchoredPosition -= new Vector2(100, 0);
                    else
                    {
                        content.anchoredPosition = tarPos;
                        sr.inertia = false;
                        recal = false;
                    }
                }
            }
        }
    }

    public void ScaleDownCover()
    {
        covers[packCount].ScaleDown();
    }

    public void ScaleUpCover()
    {
        covers[packCount].ScaleUp();
    }

    public void OnSliding()
    {
        if (Input.GetMouseButton(0))
        {
            recal = true;
            moving = false;
            inertiaTime = 0;
            sr.inertia = true;
        }
        for (int i = 0; i < covers.Length; i++)
            covers[i].SetAlpha(-content.anchoredPosition.x);
    }

    void ReadList(TextAsset list)
    {
        string data = list.text;
        System.StringSplitOptions option = System.StringSplitOptions.RemoveEmptyEntries;
        string[] lines = data.Split(new char[] { '\n' }, option);
        char[] spliter = new char[1] { ',' };

        /*
        if (!firstTime)
        {
            tempName = songNames[currentSong];
        }
        else if (lastSongName != null)
        {
            tempName = lastSongName;
        }
        currentSong = 0;*/

        packNames = new string[lines.Length];
        totalSongs = new int[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            string[] datas = lines[i].Split(spliter, option);
            packNames[i] = datas[0];
            totalSongs[i] = int.Parse(datas[1]);
        }
    }

    void CreatList()
    {
        content.sizeDelta = new Vector2(CameraAdjust.screenSize.x + 1280 * (packNames.Length - 1), 0);
        tarPos = new Vector2(packCount * -1280, 0);
        content.anchoredPosition = tarPos;
        covers = new PackCover[packNames.Length];
        for (int i = 0; i < packNames.Length; i++)
        {
            covers[i] = Instantiate(packCover, content).GetComponent<PackCover>();
            covers[i].SetCover(packNames[i], i);
        }
    }
}
