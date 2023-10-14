using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 协程移动到目标位置并停下, 并在Inspector面板有一个进度条<para/>
/// </summary>
public class TestContinuousTask : MonoBehaviour
{
    public GameObject Origin;
    public GameObject Target;
    public bool StartToggle;
    public bool MovingToggle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (StartToggle)
        {
            StartToggle = false;
            MovingToggle = true;

            StartCoroutine(MoveTask());
        }
    }


    public IEnumerator MoveTask()
    {
        while (MovingToggle)
        {
            MoveAndDetection();
            yield return new WaitForFixedUpdate();
        }
    }


    Vector3 lastDir;
    private void MoveAndDetection()
    {
        if (!MovingToggle) return;

        //配置
        var moveRate = 1/60f;

        var selfPos = Origin.transform.position;
        var targetPos = Target.transform.position;
        var laterPos = selfPos + new Vector3(moveRate, 0, 0);

        //判断方向
        var targetDir = targetPos - laterPos;
        var areOpposite = Vector3.Dot(lastDir, targetDir);
        if (areOpposite < 0)
        {
            Debug.Log("方向相反");
            MovingToggle = false;
            laterPos = targetPos;
        }
        else
        {
            //更新此次运动方向
            lastDir = targetDir;
        }

        //移动
        if(areOpposite<0) Debug.Log($"Move arrive.");
        else Debug.Log($"Move add x{moveRate}.");
        Origin.transform.position = laterPos;

    }


}
