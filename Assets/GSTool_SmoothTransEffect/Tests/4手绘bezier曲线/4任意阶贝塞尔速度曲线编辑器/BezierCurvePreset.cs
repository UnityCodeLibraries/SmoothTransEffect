using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 任意阶贝塞尔曲线预设<para/>
/// </summary>
[CreateAssetMenu(fileName = "BezierCurvePreset", menuName = "Presets/BezierCurvePreset")]
public class BezierCurvePreset : ScriptableObject
{
    public Vector3[] ControlPoints;

    private void OnValidate()
    {
        var minCount = 2;
        if (ControlPoints.Length >= minCount) return;

        var newarr = new Vector3[minCount];
        for (int i = 0; i < minCount; i++)
            if (ControlPoints.Length <= i)
                newarr[i] = Vector3.zero;
            else
                newarr[i] = ControlPoints[i];
        ControlPoints = newarr;
    }
}