using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 计算任意阶贝塞尔曲线<para/>
/// </summary>
public class BezierCurveCalculator : MonoBehaviour
{
    public static float CalculateBezierPointY(float t, Vector3[] points)
    {
        var n = points.Length - 1;
        var point = 0f;
        var u = 1 - t;
        for (int i = 0; i <= n; i++)
        {
            var y = points[i].y;
            var binomialCoefficient = i == 0 || i == n ? 1 : n;
            point += y * binomialCoefficient * Mathf.Pow(u, n - i) * MathF.Pow(t, i);
        }

        //var p = 
        //    y0 * 1 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 0) + 
        //    y1 * 3 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 1) + 
        //    y2 * 3 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 2) +
        //    y3 * 1 * Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 3);  //三次
        //var ppp = 
        //    y0 * 1 * Mathf.Pow(1 - t, 4) * Mathf.Pow(t, 0) + 
        //    y1 * 4 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 1) + 
        //    y2 * 4 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 2) +
        //    y3 * 4 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 3) +
        //    y4 * 1 * Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 4);  //四次
        //Debug.Log($"Y分量: {p}, Y增量: {p-oldPy}");oldPy = p;
        return point;
    }


}