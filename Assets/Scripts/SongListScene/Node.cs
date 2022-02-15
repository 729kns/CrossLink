using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public Text nodeInfo;
    private Vector2 setPosition;
    private LineRenderer line;
    private Vector2 dir;
    private bool moving;
    private int state;
    private bool firstTime;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        if (SwitchSceneAnimation.atListScene)
            firstTime = true;
        else
            firstTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (NodeNet.skipFrame > 0)
            return;
        transform.Translate(dir);
        switch (state)
        {
            case 1:
                if (!firstTime && Time.frameCount % 4 == 0)
                    nodeInfo.text = System.Convert.ToString(Random.Range(1000, 1000000), 16);
                if (moving)
                {
                    if (Vector2.Distance(transform.position, setPosition) <= 0.15f)
                    {
                        dir = new Vector2(Random.Range(-1, 1), Random.Range(-1, 1));
                        dir.Normalize();
                        dir = dir * 0.002f;
                        moving = false;
                        if (firstTime)
                        {
                            NodeNet.freezeCount--;
                            firstTime = false;
                        }
                    }
                }
                else if (Vector2.Distance(transform.position, setPosition) >= 0.25f)
                {
                    dir = -dir + new Vector2(Random.Range(-0.001f, 0.001f), Random.Range(-0.001f, 0.001f));
                    dir.Normalize();
                    dir = dir * 0.002f;
                }
                break;
            case -1:
                if (Time.frameCount % 4 == 0 && nodeInfo.text.Length > 0)
                    nodeInfo.text = nodeInfo.text.Substring(0, nodeInfo.text.Length - 1);
                if (Vector2.Distance(transform.position, setPosition) <= 0.1f)
                {
                    dir = setPosition;
                    transform.position = setPosition;
                    if (nodeInfo.text.Length == 0)
                    {
                        NodeNet.freezeCount++;
                        enabled = false;
                    }
                }
                break;
        }
        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.position);
    }

    public void SetLink(Vector2 pos,bool stable)
    {
        line.SetPosition(1, pos);
        if (stable)
        {
            line.startWidth = 0.2f;
            line.startColor = Color.white;
            line.endColor = Color.white;
        }
        else
        {
            line.startWidth = 0.07f;
            line.startColor = Color.gray;
            line.endColor = Color.gray;
        }
    }

    public void SetPos(Vector2 pos, bool compeleteFreeze)
    {
        setPosition = pos;
        dir = setPosition - (Vector2)transform.position;
        dir.Normalize();
        if (compeleteFreeze)
        {
            state = -1;
            dir = dir * 0.09f;
        }
        else
        {
            state = 1;
            dir = dir * 0.1f;
            moving = true;
        }
    }
}
