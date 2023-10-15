using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制二次贝塞尔物体运动<para/>
/// </summary>
public class TesCubicBezier_x : MonoBehaviour
{
    public CubicBezierProperty_x props;
    public Transform Cube;
    public Transform Cube2;
    public Transform Cube3;
    public Vector2 startCube2Point;
    public Vector2 endCube2Point;
    Vector3 realStartCube2Point, realEndCube2Point;
    public Vector2 startCube3Point;
    public Vector2 endCube3Point;
    Vector3 realStartCube3Point, realEndCube3Point;

    [Range(0, 1)]
    public float t;
    [Range(0, 1)]
    public float yt;
    [Range(0, 1)]
    public float xt;
    public float DurationScale = 2;
    public float timeScale = 0.5f;


    private void Start()
    {
        //Initialized(1);
    }


    private void FixedUpdate()
    {
        UpdateMovement(1);
    }


    private Vector3 TransCubeByT()
    {

        var t = this.t;
        var p0 = props.StartPoint.position;
        var p1 = props.ControlPoint1.position;
        var p2 = props.ControlPoint2.position;
        var p3 = props.EndPoint.position;

        //var p = (1 - t) * p0 + t * p1;  //一次
        //var p = Mathf.Pow((1 - t), 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;  //二次
        var p = Mathf.Pow(1 - t, 3) * p0 + 3 * Mathf.Pow(1 - t, 2) * t * p1 + 3 * (1 - t) * Mathf.Pow(t, 2) * p2 + Mathf.Pow(t, 3) * p3;  //三次
        return p;
    }


    float oldPx;
    private Vector3 TransCubeByVelx()
    {
        var t = this.t;
        var p0 = props.StartPoint.localPosition;
        var p1 = props.ControlPoint1.localPosition;
        var p2 = props.ControlPoint2.localPosition;
        var p3 = props.EndPoint.localPosition;
        var x0 = p0.x;
        var x1 = p1.x;
        var x2 = p2.x;
        var x3 = p3.x;

        var p = Mathf.Pow(1 - t, 3) * x0 + 3 * Mathf.Pow(1 - t, 2) * t * x1 + 3 * (1 - t) * Mathf.Pow(t, 2) * x2 + Mathf.Pow(t, 3) * x3;  //三次
        //Debug.Log($"x分量: {p}, x增量: {p - oldPx}"); oldPx = p;
        return new Vector3(p, 0 ,0);
    }


    float oldPy;
    private Vector3 TransCubeByVely()
    {
        var t = this.t;
        var p0 = props.StartPoint.localPosition;
        var p1 = props.ControlPoint1.localPosition;
        var p2 = props.ControlPoint2.localPosition;
        var p3 = props.EndPoint.localPosition;
        var y0 = p0.y;
        var y1 = p1.y;
        var y2 = p2.y;
        var y3 = p3.y;

        var pp = 
            y0 * Mathf.Pow(1 - t, 3)  +
            y1 * 3 * Mathf.Pow(1 - t, 2) * t + 
            y2 * 3 * (1 - t) * Mathf.Pow(t, 2) + 
            y3 * Mathf.Pow(t, 3);

        var p =
            y0 * 1 * Mathf.Pow(1 - t, 3) * Mathf.Pow(t, 0) +
            y1 * 3 * Mathf.Pow(1 - t, 2) * Mathf.Pow(t, 1) +
            y2 * 3 * Mathf.Pow(1 - t, 1) * Mathf.Pow(t, 2) +
            y3 * 1 * Mathf.Pow(1 - t, 0) * Mathf.Pow(t, 3);  //三次

        var ppp = BezierCurveCalculator.CalculateBezierPointY(t, new Vector3[] { p0, p1, p2, p3 });
        //Debug.Log($"任意阶公式答案对比: [p: {p}, ppp: {ppp}, 是否匹配: {p == ppp}]");
        //BezierCurveCalculator.CalculateBezierPointY(t, new Vector3[] { p0, p1, p2, p3 });
        //Debug.Log($"Y分量: {p}, Y增量: {p-oldPy}");oldPy = p;
        return new Vector3(0, ppp, 0);
    }


    private void OnDrawGizmos()
    {
        //Initialized(0);
        UpdateMovement(0);

        var thickness = 3f;
        var controlPointSize = 0.02f;

        //绘制运动曲线
        var startPoint2 = realStartCube2Point;
        var endPoint2 = realEndCube2Point;
        var startPoint3 = realStartCube3Point;
        var endPoint3 = realEndCube3Point;
        Handles.color = Color.red;
        Handles.DrawLine(startPoint2, endPoint2, thickness / 1.2f);
        Handles.DrawLine(startPoint3, endPoint3, thickness / 1.2f);

        //绘制运动线控制点
        Handles.color = Color.white;
        Handles.DrawWireDisc(startPoint2, -Vector3.forward, controlPointSize, thickness);
        Handles.DrawWireDisc(endPoint2, -Vector3.forward, controlPointSize, thickness);
        Handles.DrawWireDisc(startPoint3, -Vector3.forward, controlPointSize, thickness);
        Handles.DrawWireDisc(endPoint3, -Vector3.forward, controlPointSize, thickness);
    }


    private void Initialized(int mode)
    {
        if (mode==0 && Application.isPlaying) return;

        startCube2Point = Cube2.transform.position;
        endCube2Point = startCube2Point; endCube2Point.x += DurationScale;

        startCube3Point = Cube3.transform.localPosition;
        endCube3Point = startCube3Point; endCube3Point.y += DurationScale;
    }

    private void UpdateMovement(int mode)
    {
        if (mode == 0 && Application.isPlaying) return;

        var horiOffset = new Vector2(-0.5f, 0.875f);
        realStartCube2Point = startCube2Point * DurationScale + horiOffset;
        realEndCube2Point = endCube2Point * DurationScale + horiOffset;
        var vertiOffset = new Vector2(-0.875f, -0.5f);
        realStartCube3Point = startCube3Point * DurationScale + vertiOffset;
        realEndCube3Point = endCube3Point * DurationScale + vertiOffset;

        //贝塞尔曲线路径
        Cube.transform.position = TransCubeByT();
        //速度曲线运动路径x
        var pos = realStartCube2Point + TransCubeByVelx() * DurationScale;
        Cube2.transform.position = pos;
        xt = pos.x - horiOffset.x;
        //速度曲线运动路径y
        pos = (Vector3)realStartCube3Point + TransCubeByVely() * DurationScale;
        Cube3.transform.position = pos;
        yt = pos.y - vertiOffset.y;
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
        if(t>=1) t = 0;
        task = true;
        Debug.Log("开始任务");
        latert = t;
        while (task)
        {
            t = latert;
            latert = t+Time.fixedDeltaTime * timeScale;
            if (latert > 1)
            {
                t = 1;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        latert = 0;
        task = false;
        Debug.Log("任务结束");
    }


}


[CustomEditor(typeof(TesCubicBezier_x))]
public class MyCustomComponent4Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(25);
        TesCubicBezier_x myComponent = (TesCubicBezier_x)target;

        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton")))
        {
            if (myComponent.task) myComponent.task = false;
            else myComponent.StartTransTask();
        }
    }


}