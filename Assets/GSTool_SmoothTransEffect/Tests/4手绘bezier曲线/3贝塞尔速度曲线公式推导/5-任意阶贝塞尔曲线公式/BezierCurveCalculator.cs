using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 计算任意阶贝塞尔曲线<para/>
/// </summary>
public class BezierCurveCalculator : MonoBehaviour
{
    public static float CalculateBezierPointY(float t, Vector3[] points) => CalculateBezierPoint(t, points).y;
    public static float CalculateBezierPointX(float t, Vector3[] points) => CalculateBezierPoint(t, points).x;
    
    /// <summary>
    /// 根据给定的单位化时间分量t(0~1)从一组Bezier控制点组返回该分节点坐标p(t)
    /// </summary>
    /// <param name="t"></param>
    /// <param name="controlPoints"></param>
    /// <returns></returns>
    public static Vector3 CalculateBezierPoint(float t, Vector3[] controlPoints)
    {
        var n = controlPoints.Length - 1;
        var point = Vector3.zero;
        var u = 1 - t;
        for (int i = 0; i <= n; i++)
        {
            var p = controlPoints[i];
            var binomialCoefficient = i == 0 || i == n ? 1 : n;
            point += p * binomialCoefficient * Mathf.Pow(u, n - i) * MathF.Pow(t, i);
        }

        //var p = 
        //    p0 * 1 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 0) + 
        //    p1 * 3 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 1) + 
        //    p2 * 3 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 2) +
        //    p3 * 1 * Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 3);  //三次
        //var ppp = 
        //    p0 * 1 * Mathf.Pow(1 - t, 4) * Mathf.Pow(t, 0) + 
        //    p1 * 4 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 1) + 
        //    p2 * 4 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 2) +
        //    p3 * 4 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 3) +
        //    p4 * 1 * Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 4);  //四次
        //Debug.Log($"p分量: {p}, p增量: {p-oldPp}");oldPp = p;
        return point;
    }
}