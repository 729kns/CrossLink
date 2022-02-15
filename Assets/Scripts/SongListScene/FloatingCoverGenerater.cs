using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingCoverGenerater : MonoBehaviour
{
    public GameObject flCover;
    private string[] songNames;
    private float timeCount;
    private float interval;
    private int coverCount;
    public static bool close;

    // Update is called once per frame
    void Update()
    {
        if (close)
            return;
        timeCount -= Time.deltaTime;
        if (timeCount <= 0)
        {
            timeCount = 6 / Random.Range(songNames.Length * 0.5f, songNames.Length) + 1.5f;
            Instantiate(flCover, transform).GetComponent<Image>().sprite = Resources.Load<Sprite>("Song Background/" + songNames[coverCount]);
            coverCount++;
            if (coverCount >= songNames.Length)
                coverCount = 0;
        }
    }

    public void GetList(string[] songList)
    {
        songNames = (string[])songList.Clone();
        coverCount = 0;
        for (int i = 0; i < songNames.Length - 1; i++)
        {
            int r = Random.Range(i + 1, songNames.Length - 1);
            string temp = songNames[i];
            songNames[i] = songNames[r];
            songNames[r] = temp;
        }
    }
}
