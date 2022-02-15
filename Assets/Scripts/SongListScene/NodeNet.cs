using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NodeNet : MonoBehaviour
{
    public GameObject node;
    public Shader shader;
    public SwitchBTMain sbtm;
    public static int freezeCount;
    private Node[] nodes;
    private Material offsetMat;
    private int[] linked;
    private Vector2 offset;
    private Vector2 dir;
    private float timeCount;
    private bool ft = true;
    private int state;
    private float cCount;
    public static int skipFrame;

    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < 30; i++)
            Instantiate(node, transform);
        nodes = GetComponentsInChildren<Node>();
        offsetMat = new Material(shader);
        GetComponent<Image>().material = offsetMat;
        offsetMat.SetColor("_Color", new Color(1, 1, 1, 0.40625f));
        linked = new int[nodes.Length];
        if (SwitchSceneAnimation.atListScene)
        {
            skipFrame = 2;
            freezeCount = 30;
            state = 1;
        }
        ChangeNet();
    }

    // Update is called once per frame
    void Update()
    {
        if (skipFrame > 0)
        {
            skipFrame--;
            return;
        }
        switch (state)
        {
            case 1:
                if (freezeCount == 0)
                {
                    sbtm.SceneEnter();
                    state = 0;
                }
                break;
            case 0:
                offset += dir;
                offsetMat.SetTextureOffset("_MainTex", offset);
                timeCount -= Time.deltaTime;
                if (timeCount <= 0)
                {
                    dir = -dir + new Vector2(Random.Range(-0.001f, 0.001f), Random.Range(-0.001f, 0.001f));
                    dir.Normalize();
                    dir = dir * 0.001f;
                    timeCount = Random.Range(2.3f, 5f);
                }
                break;
            case -1:
                if (cCount < 100)
                {
                    offset += dir;
                    offsetMat.SetTextureOffset("_MainTex", offset);
                    cCount++;
                }
                else if (freezeCount == 30)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                }
                break;
        }
        for (int i = 0; i < nodes.Length; i++)
        {
            float min = 7;
            int co = -1;
            for(int j = 0; j < nodes.Length; j++)
            {
                if (j == i || linked[i] == j)
                    continue;
                float dis = Vector2.Distance(nodes[i].transform.position, nodes[j].transform.position);
                if (dis < min)
                {
                    min = dis;
                    co = j;
                }
            }
            if (co != -1)
            {
                if (min <= 2)
                    nodes[i].SetLink(nodes[co].transform.position, true);
                else
                    nodes[i].SetLink(nodes[co].transform.position, false);
                linked[co] = i;
            }
        }
    }

    public void NodeCentralize()
    {
        freezeCount = 0;
        cCount = 0;
        state = -1;
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].SetPos(Vector2.zero, true);
        dir = new Vector2((int)(offset.x + 0.5f), (int)(offset.y + 0.5f)) - offset;
        dir = dir * 0.01f;
    }

    public void ChangeNet()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-CameraAdjust.screenSize.x * 0.6f, CameraAdjust.screenSize.x * 0.6f), Random.Range(-CameraAdjust.screenSize.y * 0.6f, CameraAdjust.screenSize.y * 0.6f));
            nodes[i].SetPos(pos * transform.parent.localScale, false);
            if (ft && !SwitchSceneAnimation.atListScene)
                nodes[i].transform.position = pos * transform.parent.localScale;
        }
        if (ft)
        {
            offsetMat.SetTextureOffset("_MainTex", offset);
            dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            dir.Normalize();
            dir = dir * 0.001f;
            timeCount = Random.Range(2.3f, 5f);
            ft = false;
        }
        else
        {
            dir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            dir.Normalize();
            dir = dir * 0.035f;
            timeCount = Random.Range(2.1f, 3.3f);
        }
    }
}
