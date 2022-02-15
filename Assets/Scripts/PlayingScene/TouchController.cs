using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    class MyFinger
    {
        //储存手指的id ,-1表示不可用
        public int id = -1;
        //在type2和3时锁定的id
        public int lockedID = -1;
        public Vector2 pos;
        static private List<MyFinger> fingers = new List<MyFinger>();
        /// 手指容器
        static public List<MyFinger> Fingers
        {
            get
            {
                if (fingers.Count == 0)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        MyFinger mf = new MyFinger();
                        mf.id = -1;
                        mf.lockedID = -1;
                        mf.pos = Vector2.zero;
                        fingers.Add(mf);
                    }
                }
                return fingers;
            }
        }
    }
    private Vector2 halfScreen;
    private Vector2 screenScale;

    // Start is called before the first frame update
    void Start()
    {
        halfScreen = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        screenScale = new Vector2(1.0f * CameraAdjust.screenSize.x / Screen.width, 1.0f * CameraAdjust.screenSize.y / Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (Launcher.pause)
            return;
        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            MyFinger.Fingers[0].pos = ((Vector2)Input.mousePosition - halfScreen) * screenScale;
        }
        if (Input.GetMouseButtonUp(0))
            MyFinger.Fingers[0].id = -1;
        else if (Input.GetMouseButtonDown(0))
        {
            MyFinger.Fingers[0].id = 0;
            MyFinger.Fingers[0].lockedID = -1;
        }
        #else
        // 遍历所有的已经记录的手指
        // --掦除已经不存在的手指
        for (int i = 0; i < 5; i++)
        {
            if (MyFinger.Fingers[i].id == -1)
                continue;
            bool stillExit = false;
            for (int j = 0; j < Input.touches.Length; j++)
            {
                if (MyFinger.Fingers[i].id == Input.touches[j].fingerId)
                {
                    stillExit = true;
                    break;
                }
            }
            if (stillExit == false)
                MyFinger.Fingers[i].id = -1;
        }
        // 遍历当前的touches
        // --并检查它们在是否已经记录在AllFinger中
        // --是的话更新对应手指的状态，不是的加进去
        for (int i = 0; i < Input.touches.Length; i++)
        {
            bool stillExit = false;
            // 存在--更新对应的手指
            for (int j = 0; j < 5; j++)
            {
                if (Input.touches[i].fingerId == MyFinger.Fingers[j].id)
                {
                    stillExit = true;
                    MyFinger.Fingers[j].pos = (Input.touches[i].position - halfScreen) * screenScale;      //  可能存疑，需实机测试
                    break;
                }
            }

            // 不存在--添加新记录
            if (!stillExit)
            {
                for (int j = 0; j < 5; j++)
                {
                    //找到一个空的存储地址时，将该手指地址初始化
                    if (MyFinger.Fingers[j].id == -1)
                    {
                        MyFinger.Fingers[j].id = Input.touches[i].fingerId;
                        MyFinger.Fingers[j].lockedID = -1;
                        MyFinger.Fingers[j].pos = (Input.touches[i].position - halfScreen) * screenScale;
                        break;
                    }
                }
            }
        }
        #endif
    }

    public static bool TouchNote(int id,Vector2 targetPos)
    {
        for (int i = 0; i < 5; i++)
        {   //  先判断手指存在                     这是个自由的手指                或者      这个手指与这个note绑定了          再判断触点与目标点的距离
            if (MyFinger.Fingers[i].id != -1 && (MyFinger.Fingers[i].lockedID == -1 || MyFinger.Fingers[i].lockedID == id) && Vector2.Distance(MyFinger.Fingers[i].pos, targetPos) <= 208)
            {
              //  float dis = Vector2.Distance(MyFinger.Fingers[i].pos, targetPos);
             //   Debug.Log("dis:"+dis);
                MyFinger.Fingers[i].lockedID = id;
                return true;
            }
        }
        return false;
    }

    public static int Judge(float judgeTime, float resTime)
    {
        judgeTime = judgeTime - resTime;
        if (judgeTime >= 0.3)
            return 0;  //miss
        if (judgeTime < 0.05 && judgeTime > -0.05)   //ex-perfect
            return 3;
        else if (judgeTime < 0.12 && judgeTime > -0.12)  //perfect
            return 2;
        else if (judgeTime < 0.3 && judgeTime > 0)  //late
            return 1;
        else if (judgeTime > -0.3 && judgeTime < 0) //early
            return 4;
        else      //not in range
            return -1;
    }

    public static int JudgeEnd(float judgeTime, float resTime)
    {
        judgeTime = judgeTime - resTime;
        if (judgeTime >= 0.3)
            return 0;  //miss
        if (judgeTime < 0.1 && judgeTime > -0.1)   //ex-perfect
            return 3;
        else if (judgeTime < 0.2 && judgeTime > -0.2)  //perfect
            return 2;
        else if (judgeTime < 0.3 && judgeTime > 0)  //late
            return 1;
        else if (judgeTime > -0.3 && judgeTime < 0) //early
            return 4;
        else      //not in range
            return -1;
    }
}
