using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note1 : MonoBehaviour
{
    public CrossLine crossLine;
    public Image im;
    public Sprite dual;
    private int id;
    private Vector2 dirct;
    private Vector2 tarPos;
    private Vector2 totalDist;
    private int state;
    private float waitTime;
    private float resTime;
    private float pasTime;
    private float judgeTime;
    private CanvasRenderer cr;
    private float fadeRate = 1;
    private int judgeState;
    private bool headJudged;

    void Initialize(Note data)
    {
        transform.localPosition = Launcher.initialPos;
        id = data.id;
        cr = GetComponent<CanvasRenderer>();
        totalDist = data.distance;
        waitTime = data.waitTime;
        resTime = data.moveTime;
        dirct = data.dirct;
        tarPos = data.startPos;
        if (data.dual)
            im.sprite = dual;
        crossLine.SerParameter(tarPos, resTime + waitTime, dirct);
    }

    // Update is called once per frame
    void Update()
    {
        if (Launcher.pause)
            return;
        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }
        if (!headJudged)
        {
            judgeTime += Time.deltaTime;
//#if UNITY_EDITOR
            if (TestMode.auto)
            {
                if (judgeTime >= resTime)
                {
                    headJudged = true;
                    ScoreManager.launchEffect(tarPos, 3, false);
                    Destroy(gameObject);
                }
            }
            else
//#endif
            {
                judgeState = TouchController.Judge(judgeTime, resTime);
                if (judgeState == 0)
                {
                    headJudged = true;
                    ScoreManager.launchEffect(tarPos, judgeState, false);
                }
                else if (judgeState > 0 && TouchController.TouchNote(id, tarPos))
                {
                    headJudged = true;
                    //   Debug.Log("id:"+id+" judge:"+TouchController.Judge(judgeTime, resTime));
                    ScoreManager.launchEffect(tarPos, judgeState, false);
                    Destroy(gameObject);
                }
            }
        }
        switch (state)
        {
            case 0:
                pasTime += Time.deltaTime;
                transform.localPosition = tarPos - totalDist * (resTime - pasTime) / resTime;
                if (pasTime >= resTime)
                {
                    state = 1;
                    transform.localPosition = tarPos;
                }
                break;
            case 1:
                fadeRate -= 0.04f;
                cr.SetAlpha(fadeRate);
                if(fadeRate<=0)
                    Destroy(gameObject);
                break;
        }
    }
}
