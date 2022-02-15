using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note2 : MonoBehaviour
{
    public CrossLine crossLine;
    public Image im;
    public Sprite dual;
    public RectTransform tail;
    public GameObject touchEffect;
    private int id;
    private Vector2 dirct;
    private Vector2 tarPos;
    private Vector2 totalDist;
    private float noteLength;
    private Vector2 tailEnd;
    private int state;
    private float waitTime;
    private float resTime, endResTime;
    private float pasTime;
    private float judgeTime;
    private CanvasRenderer[] cr;
    private float fadeRate = 1;
    private int judgeState, lastJudge, realLastJudge;
    private bool headJudged, endJudged;

    void Initialize(Note data)
    {
        transform.localPosition = Launcher.initialPos;
        id = data.id;
        totalDist = data.distance;
        waitTime = data.waitTime;
        resTime = data.moveTime;
        dirct = data.dirct;
        tarPos = data.startPos;
        noteLength = data.length;
        endResTime = resTime + noteLength;
        crossLine.SerParameter(tarPos, resTime + waitTime, waitTime + resTime + noteLength, dirct);
        tail.eulerAngles = new Vector3(0, 0, dirct.y * 90);
        if (data.dual)
            im.sprite = dual;
        cr = GetComponentsInChildren<CanvasRenderer>();
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
        judgeTime += Time.deltaTime;
        if (!headJudged)
        {
//#if UNITY_EDITOR
            if (TestMode.auto)
            {
                if (judgeTime >= resTime)
                {
                    headJudged = true;
                    ScoreManager.launchEffect(tarPos, 3, false);
                }
            }
            else
//#endif
            {
                judgeState = TouchController.Judge(judgeTime, resTime);
                if (judgeState == 0 || (judgeState > 0 && TouchController.TouchNote(id, tarPos)))
                {
                    headJudged = true;
                    ScoreManager.launchEffect(tarPos, judgeState, false);
                }
            }
        }
        if (state != 0)
        {
            if (!endJudged)
            {
//#if UNITY_EDITOR
                if (TestMode.auto)
                {
                    if (judgeTime >= endResTime)
                    {
                        endJudged = true;
                        realLastJudge = 3;
                        //    ScoreManager.launchEffect(tarPos, 3);
                    }
                }
                else
//#endif
                {
                    lastJudge = TouchController.JudgeEnd(judgeTime, endResTime);
                    if (lastJudge == 0 && realLastJudge == 0)
                    {
                        endJudged = true;
                        ScoreManager.launchEffect(tarPos, lastJudge, true);
                    }
                    else if (lastJudge > 0 && TouchController.TouchNote(id, tarPos))
                    {
                        realLastJudge = lastJudge;
                    }
                }
            }
            if (!TouchController.TouchNote(id, tarPos) && !TestMode.auto)
            {
                for (int i = 0; i < cr.Length; i++)
                    cr[i].SetColor(new Color(0.7f, 0.7f, 0.7f));
            }
            else
            {
                for (int i = 0; i < cr.Length; i++)
                    cr[i].SetColor(new Color(1, 1, 1));
                if (Time.frameCount % 5 == 0)
                    Instantiate(touchEffect, transform.parent).transform.localPosition = transform.localPosition;
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
                    pasTime = resTime;
                    transform.localPosition = tarPos;
                }
                break;
            case 1:
                noteLength -= Time.deltaTime;
                if (noteLength <= 0)
                {
                    noteLength = 0;
                    state = 2;
                    if (realLastJudge > 0)
                    {
                        endJudged = true;
                        ScoreManager.launchEffect(tarPos, realLastJudge, true);
                        Destroy(gameObject);
                    }
                }
                break;
            case 2:
                fadeRate -= 0.04f;
                for (int i = 0; i < cr.Length; i++)
                    cr[i].SetAlpha(fadeRate);
                if (fadeRate <= 0)
                    Destroy(gameObject);
                break;
        }
        tailEnd = tarPos - totalDist * (resTime + noteLength - pasTime) / resTime;
        tail.localPosition = 0.5f * (tailEnd - (Vector2)transform.localPosition);
        tail.sizeDelta = new Vector2(Vector2.Distance(transform.localPosition, tailEnd) + 60, 60);
    }
}