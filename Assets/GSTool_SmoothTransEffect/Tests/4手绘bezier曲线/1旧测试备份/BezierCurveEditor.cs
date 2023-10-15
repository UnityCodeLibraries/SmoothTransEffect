using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierCurveData))]
public class BezierCurveEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BezierCurveData curveData = (BezierCurveData)target;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.LabelField("Bezier Curve Data");

        //curveData.bezierCurve.startPoint = EditorGUILayout.Vector3Field("Start Point", curveData.bezierCurve.startPoint);
        //curveData.bezierCurve.controlPoint1 = EditorGUILayout.Vector3Field("Control Point 1", curveData.bezierCurve.controlPoint1);
        //curveData.bezierCurve.controlPoint2 = EditorGUILayout.Vector3Field("Control Point 2", curveData.bezierCurve.controlPoint2);
        //curveData.bezierCurve.endPoint = EditorGUILayout.Vector3Field("End Point", curveData.bezierCurve.endPoint);

        if (EditorGUI.EndChangeCheck())
        {
            // Handle changes and apply them to your object
            // For example, you can use curveData to apply the changes to a GameObject's position, etc.
            // YourScriptContainingBezierData might contain logic to handle the curve data.
            //curveData.ApplyBezierChanges();
        }
    }
}
