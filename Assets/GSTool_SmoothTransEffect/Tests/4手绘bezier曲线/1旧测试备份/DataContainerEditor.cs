using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataContainer))]
public class DataContainerEditor : Editor
{
    public AnimationCurve curve = new AnimationCurve();
    public Vector3 startPoint;
    public Vector3 endPoint;
    public Vector3 controlPoint1;
    public Vector3 controlPoint2;
    public float x;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DataContainer data = target as DataContainer;

        data.journeyTime = data.journeyNormalizedTime * data.journeyDuration;
        startPoint = data.startPoint;
        endPoint = data.endPoint;
        controlPoint1 = data.controlPoint1;
        controlPoint2 = data.controlPoint2;
        x = data.x;
    }


    private void OnSceneGUI()
    {
        DataContainer data = target as DataContainer;

        float handleWidth = 3f;

        Handles.DrawBezier(
            data.startPoint,
            data.endPoint,
            data.controlPoint1,
            data.controlPoint2,
            Color.white,
            null,
            handleWidth
        );

        var x = this.x;
        var center = new Vector3( x, GetYFromX(x));
        // 使用Handles.DrawWireDisc来绘制圆圈
        Handles.DrawWireDisc(center, new Vector3(0, 0, -1), handleWidth/60f);
    }


    public float GetYFromX(float x)
    {
        // 对于三次贝塞尔曲线，y(t) = (1-t)^3 * y0 + 3(1-t)^2 * t * y1 + 3(1-t) * t^2 * y2 + t^3 * y3
        float t = EstimateTForX(x);
        float y = Mathf.Pow(1 - t, 3) * startPoint.y +
                  3 * Mathf.Pow(1 - t, 2) * t * controlPoint1.y +
                  3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2.y +
                  Mathf.Pow(t, 3) * endPoint.y;

        return y;
    }

    // 根据给定的x值估算t值
    private float EstimateTForX(float x)
    {
        // 这个方法基于简单的二分法，可以快速估算t值
        float minT = 0f;
        float maxT = 1f;
        float t = (minT + maxT) / 2;

        for (int i = 0; i < 20; i++) // 迭代次数可以根据需要调整
        {
            float xEstimate = CalculateXFromT(t);

            if (Mathf.Approximately(xEstimate, x))
            {
                break;
            }

            if (xEstimate < x)
            {
                minT = t;
            }
            else
            {
                maxT = t;
            }

            t = (minT + maxT) / 2;
        }

        return t;
    }

    // 根据给定的t值计算x值
    private float CalculateXFromT(float t)
    {
        float x = Mathf.Pow(1 - t, 3) * startPoint.x +
                  3 * Mathf.Pow(1 - t, 2) * t * controlPoint1.x +
                  3 * (1 - t) * Mathf.Pow(t, 2) * controlPoint2.x +
                  Mathf.Pow(t, 3) * endPoint.x;

        return x;
    }


}
