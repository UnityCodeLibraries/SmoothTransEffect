using System;
using System.Collections;
using UnityEngine;


/// <summary>
/// 根据距离的贝塞尔加速度与减速度: <para/>
/// </summary>
public class TestContinuousTask_3 : MonoBehaviour
{
    [Header("引用")]
    public Transform Origin;
    public Transform Target;
    public BezierCurvePreset bezier;
    public bool start;
    public bool task;
    public float timescale = 3f;

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
            StartCoroutine(Task());
        }
    }

    IEnumerator Task()
    {
        if (task) yield return null;
        
        task = true;
        StartTime = Time.time;
        StartPos = Origin.transform.position;
        while (task)
        {
            CurrentTime = Time.time;
            NormalizedTime = CurrentTime - StartTime;
            var selfPos = Origin.transform.position;
            var targetPos = Target.transform.position;
            var rate = BezierCurveCalculator.CalculateBezierPointX(NormalizedTime / timescale, bezier.ControlPoints);
            //if ((NormalizedTime = CurrentTime - StartTime) > timescale)
            if (selfPos == targetPos)
            {
                task = false;
                break;
            }
            Origin.transform.position = Vector3.Lerp(selfPos, targetPos, rate);
            yield return new WaitForFixedUpdate();
        }
    }
}
