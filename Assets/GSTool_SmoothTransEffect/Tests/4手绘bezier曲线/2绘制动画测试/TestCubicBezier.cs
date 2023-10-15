using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制三次贝塞尔曲线图示<para/>
/// </summary>
public class TestCubicBezier : MonoBehaviour
{
    public CubicBezierProperty bezier;
    public Transform Cube;
    public float journeyDuration = 3f;
    public float journeyTime;
    [Range(0, 1)]
    public float journeyNormalizedTime;
    public Vector2 startCubePoint;
    public Vector2 endCubePoint;

    public bool start;
    public bool TransTask;


    private void Start()
    {
        startCubePoint = Cube.transform.position;
        endCubePoint = Cube.transform.position; endCubePoint.x += 1;
    }


    private void FixedUpdate()
    {
        journeyTime = journeyNormalizedTime * journeyDuration;

        if (start)
        {
            start = false;
            TransTask = true;
        }

        if (TransTask)
        {
            if (journeyTime > journeyDuration)
            {
                Debug.Log("TransTask completed");
                journeyNormalizedTime = 0;
                TransTask = false;
                return;
            }

            journeyNormalizedTime += Time.fixedDeltaTime /3f;

            TransCube(journeyNormalizedTime, bezier);
        }

    }

    Vector3 EvaluateCubicBezier(CubicBezierProperty bezier, float t)
    {
        Vector2 p0 = bezier.StartPoint.position;
        Vector2 p1 = bezier.ControlPoint1.position;
        Vector2 p2 = bezier.ControlPoint2.position;
        Vector2 p3 = bezier.EndPoint.position;

        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0; // (1-t)^3 * P0
        p += 3 * uu * t * p1; // 3 * (1-t)^2 * t * P1
        p += 3 * u * tt * p2; // 3 * (1-t) * t^2 * P2
        p += ttt * p3; // t^3 * P3

        return p;
    }


    float oldPointy;
    Vector2 targetCurvePoint = new Vector2(0.5f, 0.5f);
    Vector2 startCurvePoint = new Vector2(0, 0);
    // 在 TransCube 中使用 EvaluateCubicBezier 计算 x 和 y 分量
    private void TransCube(float journeyNormalizedTime, CubicBezierProperty bezier)
    {
        var t = journeyNormalizedTime;
        var point = EvaluateCubicBezier(bezier, t);
        var currentCurvePoint = point;
        var distance = Vector2.Distance(targetCurvePoint, currentCurvePoint);
        Debug.Log($"distance: {distance}");


        var trans = Vector3.zero;
        //trans.x = distance;
        trans.x = point.x+0.5f;
        Debug.Log(point.y - oldPointy);
        oldPointy = point.y;
        Cube.transform.localPosition = trans*10;

        // 现在，x 和 y 包含了 t 时刻的贝塞尔曲线上的位置
    }


    private void OnDrawGizmos()
    {
        var controlPointSize = 0.02f;
        var startPoint = startCubePoint;
        var endPoint = endCubePoint;
        var thickness = 3f;
        Handles.color = Color.red;
        Handles.DrawLine(startPoint, endPoint, thickness);

        Handles.color = Color.white;
        Handles.DrawWireDisc(startPoint, -Vector3.forward, controlPointSize, thickness);
        Handles.DrawWireDisc(endPoint, -Vector3.forward, controlPointSize, thickness);
    }


}
