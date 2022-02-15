using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Title : MonoBehaviour
{
    private Text t;
    private RectTransform rt;
    private string newTitle = "";
    private float flashCount;
    private bool flash;

    void Awake()
    {
        t = GetComponent<Text>();
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (t.text.Length < newTitle.Length && Time.frameCount % 3 == 0)
            t.text = newTitle.Substring(0, t.text.Length + 1);
        else
        {
            flashCount -= Time.deltaTime;
            if (flashCount <= 0)
            {
                if (flash)
                    t.text = newTitle + "_";
                else
                    t.text = newTitle;
                flash = !flash;
                flashCount = 1;
            }
        }
    }

    public void GetTitle(string set)
    {
        if (newTitle == set)
            return;
        newTitle = set;
        t.text = "";
        flashCount = 1;
        flash = true;
        rt.anchoredPosition = new Vector2(8.75f * (24-System.Text.Encoding.Default.GetByteCount(newTitle)), 0);
    }
}
