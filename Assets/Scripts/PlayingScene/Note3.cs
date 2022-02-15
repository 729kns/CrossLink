using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note3 : MonoBehaviour
{
    public CrossLine startCL, endCL;
    public Image im;
    public Sprite dual;
    public RectTransform tail;
    public GameObject touchEffect;
    private int id;
    private float noteLength;
    private Vector2 dirct;
    private Vector2 tarPos, slidePos;
    private Vector2 totalDist;
    private Vector2 tailEnd;
    private Vector2 tarSize, scaleSpeed;
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
        slidePos = data.endPos;
        noteLength = data.length;
        endResTime= resTime + noteLength;
        startCL.SerParameter(tarPos, resTime + waitTime, waitTime + resTime + noteLength, dirct);
        endCL.SerParameter(slidePos, waitTime + resTime + noteLength, false);
        cr = GetComponentsInChildren<CanvasRenderer>();
        tail.localPosition = 0.5f * (tarPos + slidePos) - (Vector2)transform.localPosition;
        tarSize = new Vector2(Vector2.Distance(tarPos, slidePos) + 40, 40);
        scaleSpeed = new Vector2(Vector2.Distance(tarPos, slidePos) / resTime, 0);
        tail.eulerAngles = new Vector3(0, 0, Vector2.Angle(slidePos - tarPos, (tarPos.y > slidePos.y ? Vector2.left : Vector2.right)));
        tail.gameObject.SetActive(false);
        if (data.dual)
            im.sprite = dual;
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
        if (!tail.gameObject.activeSelf)
            tail.gameObject.SetActive(true);
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
                        //     ScoreManager.launchEffect(slidePos, 3);
                    }
                }
                else
//#endif
                {
                    lastJudge = TouchController.JudgeEnd(judgeTime, endResTime);
                    if (lastJudge == 0 && realLastJudge == 0)
                    {
                        endJudged = true;
                        ScoreManager.launchEffect(slidePos, lastJudge, true);
                    }
                    else if (lastJudge > 0 && TouchController.TouchNote(id, transform.localPosition))
                    {
                        realLastJudge = lastJudge;
                    }
                }
            }
            if (!TouchController.TouchNote(id, transform.localPosition) && !TestMode.auto)
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
                    pasTime = 0;
                    state = 1;
                    tail.sizeDelta = tarSize;
                }
                tail.localPosition = 0.5f * (startCL.transform.localPosition + endCL.transform.localPosition) - transform.localPosition;
                tail.sizeDelta += scaleSpeed * Time.deltaTime;
                break;
            case 1:
                pasTime += Time.deltaTime;
                transform.localPosition = tarPos + (slidePos - tarPos) * pasTime / noteLength;
                startCL.move(transform.localPosition);
                tail.localPosition = 0.5f * (startCL.transform.localPosition + endCL.transform.localPosition) - transform.localPosition;
                tail.sizeDelta = new Vector2(Vector2.Distance(startCL.transform.localPosition, endCL.transform.localPosition) + 40, 40);
                if (pasTime >= noteLength)
                {
                    transform.localPosition = slidePos;
                    Destroy(startCL.gameObject);
                    tail.localPosition = slidePos - (Vector2)transform.localPosition;
                    tail.sizeDelta = new Vector2(40, 40);
                    state = 2;
                    if (realLastJudge > 0)
                    {
                        endJudged = true;
                        ScoreManager.launchEffect(slidePos, realLastJudge, true);
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

        //   tailEnd = tarPos - totalDist * (resTime + noteLength - pasTime) / (resTime);
    }
}