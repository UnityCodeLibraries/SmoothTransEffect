using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制二次贝塞尔物体运动<para/>
/// </summary>
public class TesCubicBezier : MonoBehaviour
{
    public CubicBezierProperty props;
    public Transform Cube;
    public Transform Cube2;
    public Vector3 CubeRealPos;
    [Range(0, 1)]
    public float t;
    public float DurationScale = 2;

    Vector2 startCubePoint;
    Vector2 endCubePoint;


    private void Start()
    {
        startCubePoint = Cube.transform.position;
        endCubePoint = Cube.transform.position; endCubePoint.x += DurationScale;
    }


    private void FixedUpdate()
    {
        CubeRealPos = Cube.transform.position;

        UpdateMovement();
    }


    private void UpdateMovement()
    {
        //贝塞尔路径
        Cube.transform.position = TransCubeByT();

        //速度曲线运动路径
        var pos = (Vector3)startCubePoint + TransCubeByVel() * DurationScale; pos.x += 0.5f * DurationScale;
        Cube2.transform.position = pos;
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


    private Vector3 TransCubeByVel()
    {

        var t = this.t;
        var p0 = props.StartPoint.position;
        var p1 = props.ControlPoint1.position;
        var p2 = props.ControlPoint2.position;
        var p3 = props.EndPoint.position;
        var y0 = p0.y;
        var y1 = p1.y;
        var y2 = p2.y;
        var y3 = p3.y;

        var p = Mathf.Pow(1 - t, 3) * y0 + 3 * Mathf.Pow(1 - t, 2) * t * y1 + 3 * (1 - t) * Mathf.Pow(t, 2) * y2 + Mathf.Pow(t, 3) * y3;  //三次
        return new Vector3(p, 0 ,0);
    }


    private void OnDrawGizmos()
    {
        var thickness = 3f;
        var controlPointSize = 0.02f;

        var startPoint = startCubePoint;
        var endPoint = endCubePoint;
        Handles.color = Color.red;
        Handles.DrawLine(startPoint, endPoint, thickness / 1.2f);

        Handles.color = Color.white;
        Handles.DrawWireDisc(startPoint, -Vector3.forward, controlPointSize, thickness);
        Handles.DrawWireDisc(endPoint, -Vector3.forward, controlPointSize, thickness);
    }


    public bool task;
    float latert;
    public void StartTransTask()
    {
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
            latert = t+Time.fixedDeltaTime;
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


[CustomEditor(typeof(TesCubicBezier))]
public class MyCustomComponent3Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(25);
        TesCubicBezier myComponent = (TesCubicBezier)target;

        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton")))
        {
            if (myComponent.task) myComponent.task = false;
            else myComponent.StartTransTask();
        }
    }


}