using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 任意阶贝塞尔曲线预设场景图形化编辑器<para/>
/// </summary>
public class BezierCurveSceneEditor : MonoBehaviour
{
    [Header("引用")]
    public BezierCurvePreset preset;
    public Transform ControlPointsBrush;

    [Header("配置")]
    public float timeScale = 0.5f;
    public Vector3 cubeSize = new Vector3(0.2f, 0.2f, 0.2f);
    public Vector3 cube1Offset = new Vector3(0, 0);
    public Vector3 cube2Offset = new Vector3(0, 1.375f);

    [Header("实时")]
    [Range(0, 1)]
    public float currentT;
    [Range(0, 1)]
    public float XT;
    
    Vector3[] points;
    Vector3 point1 = new Vector3(1, 1, 1);


    [ContextMenu("RefreshControlPointBrush")]
    void RefreshControlPointBrush()
    {
        for (int i = 0; i < points.Length; i++)
        {
            var name = "ControlPoint" + i;
            var n = ControlPointsBrush.childCount -1;
            Transform obj = null;
            if (i > n)
            {
                obj = new GameObject(name).transform;
                obj.SetParent(ControlPointsBrush);
            }
            else
            {
                obj = ControlPointsBrush.GetChild(i);
            }
            obj.localPosition = points[i];
        }
    }

    [ContextMenu("RefreshControlPointsData")]
    void RefreshControlPointsData()
    {
        for (int i = 0; i < points.Length; i++)
        {
            var name = "ControlPoint" + i;
            var obj = ControlPointsBrush.Find(name);
            if (!obj) continue;
            preset.ControlPoints[i] = obj.localPosition;
        }
    }

    private void OnGUI()
    {
        //GUILayout.Button("hhhhh");
    }


    int curvature = 90;
    Vector3[] pathPoints;
    private void OnDrawGizmos()
    {
        if (DetectNull(preset, "", LogType.Error))
        {
            this.gameObject.SetActive(false);
            return;
        }
        Initialized();
        UpdateDatas();
        var center = transform.position;
        var uiCenter = center + new Vector3(0.5f, 0.5f);
        var facing = -Vector3.forward;
        var controlPointRadius = 0.03f;
        var thickness = 2.5f;
        var journeyLength = 1;

        //绘制曲线界面
        //绘制控制点
        foreach (var i in points)
        {
            var pos = center+i;
            Handles.DrawWireDisc(pos, facing, controlPointRadius, thickness);
        }

        //绘制贝塞尔曲线
        Handles.color = Color.red;
        for (int i = 0; i < curvature; i++)
        {
            float t = (float)i / (curvature - 1);
            pathPoints[i] = BezierCurveCalculator.CalculateBezierPoint(t, points);
            pathPoints[i] += center;
            if(i!=0)
                Handles.DrawLine(pathPoints[i-1], pathPoints[i], thickness);
        }

        //绘制背景
        Handles.color = Color.white;
        Handles.DrawWireCube(uiCenter, point1);


        //绘制运动界面
        //控制点
        var startPoint = center + cube2Offset;
        var endPoint = startPoint; endPoint.x += journeyLength;
        Handles.DrawWireDisc(startPoint, facing, controlPointRadius, thickness);
        Handles.DrawWireDisc(endPoint, facing, controlPointRadius, thickness);

        //行程线
        Handles.color = Color.red;
        Handles.DrawLine(startPoint, endPoint, thickness);


        //绘制Cube
        //curve
        ColorUtility.TryParseHtmlString("#E9E966BB", out Color hexColorYellow);
        Gizmos.color = hexColorYellow;
        var p1 = BezierCurveCalculator.CalculateBezierPoint(currentT, points);
        var pCube1 = center + p1 + cube1Offset;
        Gizmos.DrawCube(pCube1, cubeSize);
        //movement
        //var p2 = p1; p2.y = 0;
        XT = p1.x;  //更新XT分量
        //var pCube2 = center + cube2Offset + p2;
        var startMovementPos = center + cube2Offset;
        var targetMovementPos = startMovementPos;
        targetMovementPos.x += journeyLength;
        var currentMovementPos = Vector3.Lerp(startMovementPos, targetMovementPos, XT);
        Gizmos.DrawCube(currentMovementPos, cubeSize);
    }


    /// <summary>
    /// 空指针检测, 用于检测配置
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="mes"></param>
    /// <param name="logType"></param>
    private bool DetectNull<T>(T obj, string mes="", LogType logType=LogType.Log)
    {
        if (obj == null)
        {
            if (mes == null || mes == "") mes = $"{typeof(T)}变量为空, 请检查配置";
            var logMes = $"[异常-{GetType().Name}]: {mes}.";
            switch (logType)
            {
                case LogType.Error:
                    Debug.LogError(logMes);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(logMes);
                    break;
                case LogType.Log:
                    Debug.Log(logMes);
                    break;
                default:
                    Debug.LogError("错误的LogType类型");
                    break;
            }
            return true;
        }
        return false;
    }


    private void Initialized()
    {

        if (pathPoints == null)
        {
             pathPoints = new Vector3[curvature];
        }

        if(ControlPointsBrush == null)
        {
            //ControlPointsBrush = transform.Find("ControlPoints");
            ControlPointsBrush = transform;
        }
    }

    private void UpdateDatas()
    {
        points = preset.ControlPoints;

        RefreshControlPointsData();
    }


    public bool task;
    float latert;
    public void StartTransTask()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("运行后再开始任务.");
            return;
        }
        if (task) return;
        StartCoroutine(TransTask());
    }
    IEnumerator TransTask()
    {
        if (currentT >= 1) currentT = 0;
        task = true;
        Debug.Log("开始任务");
        latert = currentT;
        while (task)
        {
            currentT = latert;
            latert = currentT + Time.fixedDeltaTime * timeScale;
            if (latert > 1)
            {
                currentT = 1;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        latert = 0;
        task = false;
        Debug.Log("任务结束");
    }


}


[CustomEditor(typeof(BezierCurveSceneEditor))]
public class BezierCurveSceneEditorDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(25);
        BezierCurveSceneEditor editor = (BezierCurveSceneEditor)target;

        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton")))
        {
            if (editor.task) editor.task = false;
            else editor.StartTransTask();
        }
    }


}