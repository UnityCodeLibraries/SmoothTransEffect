using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制三次贝塞尔曲线控制点图示<para/>
/// </summary>
public class CubicBezierProperty_y : MonoBehaviour
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
        var thickness = 3f;
        var controlPointSize = 0.02f;

        // 绘制三次贝塞尔曲线
        Vector3 startPoint = StartPoint.position;
        Vector3 controlPoint1 = ControlPoint1.position;
        Vector3 controlPoint2 = ControlPoint2.position;
        Vector3 endPoint = EndPoint.position;
        Handles.DrawBezier(
            startPoint,
            endPoint,
            controlPoint1,
            controlPoint2,
            Color.red,
            null,
            thickness*1.2f
            );

        //绘制控制点
        Handles.color = Color.white;
        Transform[] points = { StartPoint, EndPoint, ControlPoint1, ControlPoint2 };
        foreach (var i in points)
        {
            Handles.DrawWireDisc(i.position, -Vector3.forward, controlPointSize, thickness);
        }
    }

    Vector3 CalculateCubicBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }


}
