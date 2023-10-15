using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制三次贝塞尔曲线图示<para/>
/// </summary>
public class OldCubicBezierProperty : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public Transform ControlPoint1;
    public Transform ControlPoint2;


    private void Start()
    {
    }


    private void OnDrawGizmos()
    {
        InitializePoints();
        var thickness = 3f;
        Handles.DrawBezier(
            StartPoint.position,
            EndPoint.position,
            ControlPoint1.position,
            ControlPoint2.position,
            Color.red,
            null,
            thickness
            );

        var controlPointSize = 0.02f;
        Handles.color = Color.white;
        Transform[] points = { StartPoint, EndPoint, ControlPoint1, ControlPoint2 };
        foreach (var i in points)
        {
            Handles.DrawWireDisc(i.position, -Vector3.forward, controlPointSize, thickness/1.5f);
        }
    }

    private void InitializePoints()
    {
        if (StartPoint == null)
        {
            StartPoint = transform.Find("StartPoint");
            EndPoint = transform.Find("EndPoint");
            ControlPoint1 = transform.Find("ControlPoint1");
            ControlPoint2 = transform.Find("ControlPoint2");
        }
    }


}
