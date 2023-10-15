using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 绘制线性(一次)贝塞尔物体运动<para/>
/// </summary>
public class TestLinearBezier : MonoBehaviour
{
    public LinearBezierProperty props;
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
        Cube.transform.position = TransCubeByT();

        var pos = (Vector3)startCubePoint + TransCubeByVel(); pos.x += 0.5f;
        Cube2.transform.position = pos;
    }


    private Vector3 TransCubeByT()
    {

        var t = this.t;
        var p0 = props.StartPoint.position;
        var p1 = props.EndPoint.position;

        var p = (1 - t) * p0 + t * p1;
        return p;
    }


    private Vector3 TransCubeByVel()
    {

        var t = this.t;
        var p0 = props.StartPoint.position;
        var p1 = props.EndPoint.position;
        var y0 = p0.y;
        var y1 = p1.y;

        var p = (1 - t) * y0 + t * y1;
        return new Vector3(p, 0 ,0);
    }


    private void OnDrawGizmos()
    {
        var controlPointSize = 0.02f;
        var startPoint = startCubePoint;
        var endPoint = endCubePoint;
        var thickness = 3f;
        Handles.color = Color.red;
        Handles.DrawLine(startPoint, endPoint, thickness);

        Handles.color = Color.white;
        Handles.DrawWireDisc(startPoint, -Vector3.forward, controlPointSize, thickness);
        Handles.DrawWireDisc(endPoint, -Vector3.forward, controlPointSize, thickness);
    }


    public bool task;
    float lastT;
    public void StartTransTask()
    {
        if (task) return;
        StartCoroutine(TransTask());
    }
    IEnumerator TransTask()
    {
        if(t>=1) t = lastT = 0;
        task = true;
        Debug.Log("开始任务");
        while (t < 1 && task)
        {
            if (lastT != t) break;

            lastT = t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        task = false;
        Debug.Log("任务结束");
    }


}


[CustomEditor(typeof(TestLinearBezier))]
public class MyCustomComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(25);
        TestLinearBezier myComponent = (TestLinearBezier)target;

        if (GUILayout.Button(EditorGUIUtility.IconContent("PlayButton")))
        {
            if (myComponent.task) myComponent.task = false;
            else myComponent.StartTransTask();
        }
    }


}