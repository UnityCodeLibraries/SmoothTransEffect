using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制二次贝塞尔物体运动<para/>
/// </summary>
public class TesQuadraticBezier : MonoBehaviour
{
    public QuadraticBezierProperty props;
    public Transform Cube;
    public Transform Cube2;
    public Vector3 CubeRealPos;
    public Vector2 startCubePoint;
    public Vector2 endCubePoint;
    [Range(0, 1)]
    public float t;


    private void Start()
    {
        startCubePoint = Cube.transform.position;
        endCubePoint = Cube.transform.position; endCubePoint.x += 1;
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
        var pos = (Vector3)startCubePoint + TransCubeByVel(); pos.x += 0.5f;
        Cube2.transform.position = pos;
    }


    private Vector3 TransCubeByT()
    {

        var t = this.t;
        var p0 = props.StartPoint.position;
        var p1 = props.ControlPoint1.position;
        var p2 = props.EndPoint.position;

        var p = Mathf.Pow((1 - t), 2) *p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
        return p;
    }


    private Vector3 TransCubeByVel()
    {

        var t = this.t;
        var p0 = props.StartPoint.position;
        var p1 = props.ControlPoint1.position;
        var p2 = props.EndPoint.position;
        var y0 = p0.y;
        var y1 = p1.y;
        var y2 = p2.y;

        var p = Mathf.Pow((1 - t), 2) * y0 + 2 * (1 - t) * t * y1 + Mathf.Pow(t, 2) * y2;
        return new Vector3(p, 0 ,0);
    }


    private void OnDrawGizmos()
    {
        var controlPointSize = 0.02f;
        var startPoint = startCubePoint;
        var endPoint = endCubePoint;
        var thickness = 3f;
        Handles.color = Color.red;
        Handles.DrawAAPolyLine(thickness*1.8f, startPoint, endPoint);

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
        if(t>=1) t = latert = 0;
        task = true;
        Debug.Log("开始任务");
        while (latert < 1 && task)
        {
            t = latert;
            latert += Time.fixedDeltaTime;
            if(latert>1) t = 1;
            yield return new WaitForFixedUpdate();
        }
        //latert = 0;
        task = false;
        Debug.Log("任务结束");
    }


}


[CustomEditor(typeof(TesQuadraticBezier))]
public class MyCustomComponent2Editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(25);
        TesQuadraticBezier myComponent = (TesQuadraticBezier)target;

        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton")))
        {
            if (myComponent.task) myComponent.task = false;
            else myComponent.StartTransTask();
        }
    }


}