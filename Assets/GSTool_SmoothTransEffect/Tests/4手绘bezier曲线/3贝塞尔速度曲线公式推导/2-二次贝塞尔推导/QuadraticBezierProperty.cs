using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制二次贝塞尔曲线控制点图示<para/>
/// </summary>
public class QuadraticBezierProperty : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public Transform ControlPoint1;


    private void Start()
    {
    }


    private void OnDrawGizmos()
    {
        // 绘制二次贝塞尔曲线
        var thickness = 3f;
        Handles.color = Color.red;


        Vector3 startPoint = StartPoint.position;
        Vector3 controlPoint = ControlPoint1.position;
        Vector3 endPoint = EndPoint.position;
        int numberOfPoints = 90;
        Vector3 lastPoint = startPoint;
        for (int i = 1; i <= numberOfPoints; i++)
        {
            float t = i / (float)numberOfPoints;
            Vector3 point = CalculateQuadraticBezierPoint(startPoint, controlPoint, endPoint, t);
            Handles.DrawAAPolyLine(thickness*1.8f, lastPoint, point);
            lastPoint = point;
        }

        //绘制控制点
        var controlPointSize = 0.02f;
        Handles.color = Color.white;
        Transform[] points = { StartPoint, EndPoint, ControlPoint1 };
        foreach (var i in points)
        {
            Handles.DrawWireDisc(i.position, -Vector3.forward, controlPointSize, thickness);
        }
    }

    Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }


}
