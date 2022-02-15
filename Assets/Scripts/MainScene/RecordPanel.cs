using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RecordPanel : MonoBehaviour
{
    public Text[] cfts;
    private string[] cftInfos = new string[3];
    private int curPack = -1;

    void Update()
    {
        for (int i = 0; i < cfts.Length; i++)
        {
            if (cfts[i].text.Length < cftInfos[i].Length && Time.frameCount % 4 == 0)
                cfts[i].text = cftInfos[i].Substring(0, cfts[i].text.Length + 1);
        }
    }

    public void GetRecord(int total)
    {
        if (curPack == PackList.packCount)
            return;
        curPack = PackList.packCount;
        for (int i = 0; i < cftInfos.Length; i++)
        {
            cfts[i].text = "";
            int c = PlayerPrefs.GetInt(PackList.packCount + "_" + i + "_c", 0);
            int f = PlayerPrefs.GetInt(PackList.packCount + "_" + i + "_f", 0);
            if (f == total)
                cfts[i].color = new Color(1, 0.75f, 0);
            else if (c == total)
                cfts[i].color = new Color(0, 1, 1);
            else
                cfts[i].color = Color.white;
            cftInfos[i] = string.Format("{0}/{1}/{2}", c, f, total);
        }
    }
}
