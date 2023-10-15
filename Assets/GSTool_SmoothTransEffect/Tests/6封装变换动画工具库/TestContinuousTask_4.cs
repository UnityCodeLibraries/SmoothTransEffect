using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 使用变换动画封装库 <para/>
/// </summary>
public class TestContinuousTask_4 : MonoBehaviour
{
    [Header("引用")]
    public Transform Origin;
    public Transform Target;
    public BezierCurvePreset bezier;
    public bool start;
    public bool task;
    public float timeScale = 3f;
    public TransTaskUtils.Mode mode;

    public Vector3 StartPos;
    public float StartTime, CurrentTime, NormalizedTime;


    private void Start()
    {
        Origin = transform.GetChild(0);
        Target = transform.GetChild(1);
    }


    private void Update()
    {
        if (start)
        {
            start = false;
            StartTransTask();
        }
    }


    private void StartTransTask()
    {
        var selfTrans = Origin;
        var targetTrans = Target;
        var timeScale = this.timeScale;
        var points = bezier.ControlPoints;
        StartCoroutine(TransTaskUtils.TranslationTask(selfTrans, targetTrans, timeScale, mode));
    }


}
