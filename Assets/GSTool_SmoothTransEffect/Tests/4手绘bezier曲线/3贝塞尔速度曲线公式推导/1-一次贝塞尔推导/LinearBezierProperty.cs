using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制线性(一次)贝塞尔曲线控制点图示<para/>
/// </summary>
public class LinearBezierProperty : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;


    private void Start()
    {
    }


    private void OnDrawGizmos()
    {
        InitializePoints();

        //绘制控制线
        var thickness = 3f;
        Handles.color = Color.red;
        Handles.DrawLine(StartPoint.position, EndPoint.position, thickness);

        //绘制控制点
        var controlPointSize = 0.02f;
        Handles.color = Color.white;
        Transform[] points = { StartPoint, EndPoint };
        foreach (var i in points)
        {
            Handles.DrawWireDisc(i.position, -Vector3.forward, controlPointSize, thickness);
        }
    }

    private void InitializePoints()
    {
        if (StartPoint == null)
        {
            StartPoint = transform.Find("StartPoint");
            EndPoint = transform.Find("EndPoint");
        }
    }


}
