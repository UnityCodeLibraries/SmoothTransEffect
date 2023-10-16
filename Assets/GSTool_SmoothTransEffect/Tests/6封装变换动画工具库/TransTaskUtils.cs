using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 封装变换动画工具集: <para/>
/// </summary>
public class TransTaskUtils
{

    /// <summary>
    /// 预设效果模式Id: 渐入渐出效果1
    /// </summary>
    public enum Mode
    {
        LINEAR, EASE, EASE_IN, EASE_IN_OUT1
    }

    /// <summary>
    /// 预设效果BezierPoints
    /// </summary>
    public static IDictionary<Mode, Vector3[]> Effects = new Dictionary<Mode, Vector3[]>()
    {
        { Mode.LINEAR, LoadEffectData("Assets/GSTool_SmoothTransEffect/Tests/6封装变换动画工具库/LINEAR.asset") },
        { Mode.EASE, LoadEffectData("Assets/GSTool_SmoothTransEffect/Tests/6封装变换动画工具库/EASE.asset") },
        { Mode.EASE_IN, LoadEffectData("Assets/GSTool_SmoothTransEffect/Tests/6封装变换动画工具库/EASE_IN.asset") },
};


    /// <summary>
    /// 加载Asset资源
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    private static Vector3[] LoadEffectData(string assetPath)
    {
        // 加载 ScriptableObject
        BezierCurvePreset effectData = AssetDatabase.LoadAssetAtPath<BezierCurvePreset>(assetPath);

        if (effectData != null)
        {
            return effectData.ControlPoints;
        }
        else
        {
            Debug.LogError("Failed to load EffectData from path: " + assetPath);
            return null; // 或者采取其他错误处理措施
        }
    }


    /// <summary>
    /// 使用预设效果进行位移变换
    /// </summary>
    /// <param name="selfTrans"></param>
    /// <param name="targetTrans"></param>
    /// <param name="timeScale"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public IEnumerator TranslationTask(Transform selfTrans, Transform targetTrans, float timeScale, Mode mode = Mode.LINEAR)
    {
        if (!Effects.ContainsKey(mode)) yield return null;
        yield return TranslationTask(selfTrans, targetTrans, timeScale, Effects[mode]);
    }

    /// <summary>
    /// 位移变换核心函数逻辑
    /// </summary>
    /// <param name="selfTrans"></param>
    /// <param name="targetTrans"></param>
    /// <param name="timeScale"></param>
    /// <param name="controlPoints"></param>
    /// <returns></returns>
    public IEnumerator TranslationTask(Transform selfTrans, Transform targetTrans, float timeScale, Vector3[] controlPoints)
    {
        var startTime = Time.time;
        var startPos = selfTrans.position;
        while (true)
        {
            var currentTime = Time.time;
            var journeyTime = currentTime - startTime;
            if (journeyTime > timeScale) journeyTime = timeScale;

            var selfPos = selfTrans.position;
            var targetPos = targetTrans.position;
            var rate = BezierCurveCalculator.CalculateBezierPointX(journeyTime / timeScale, controlPoints);
            selfTrans.position = Vector3.Lerp(startPos, targetPos, rate);

            if (selfPos == targetPos)
            {
                selfTrans.position = targetPos;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }


    /// <summary>
    /// 无极加速
    /// </summary>
    /// <param name="selfTrans"></param>
    /// <param name="targetTrans"></param>
    /// <param name="journeyDuration"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public IEnumerator TranslationTask2(Transform selfTrans, Transform targetTrans, float journeyDuration, Mode mode = Mode.LINEAR)
    {
        if (!Effects.ContainsKey(mode)) yield return null;
        var controlPoints = Effects[mode];
        var normalizedTime = 0f;
        var startPos = selfTrans.position;
        var startVel = 0f;
        var currentVel = 0f;
        var velMultiple = 1f;
        var oldDir = Vector3.zero;
        var oldNormaliz = 0f;
        var oldAccelerationTimeRate = Vector3.zero;
        while (true)
        {
            var stepLength = Time.fixedDeltaTime / journeyDuration;
            //Debug.Log($"stepLength: {stepLength}");

            normalizedTime += stepLength;
            if (normalizedTime > journeyDuration) normalizedTime = journeyDuration;

            var currentAccelerationTimeRate = BezierCurveCalculator.CalculateBezierPoint(normalizedTime, controlPoints);
            var accelerationTimeRate = currentAccelerationTimeRate - oldAccelerationTimeRate;
            var k = accelerationTimeRate.x==0?0:accelerationTimeRate.y / accelerationTimeRate.x;
            oldAccelerationTimeRate = currentAccelerationTimeRate;

            var acceleration = k * velMultiple;

            Debug.Log($"acceleration: {acceleration}");

            var selfPos = selfTrans.position;
            var targetPos = targetTrans.position;
            var currentDir = (targetPos - selfPos).normalized;

            var transAmount = acceleration * currentDir;
            var laterPos = selfPos + transAmount;
            var laterDir = (targetPos - laterPos).normalized;

            if (Vector3.Dot(currentDir, laterDir) < 0 || normalizedTime >= journeyDuration)
            {
                selfTrans.position = targetPos;
                break;
            }

            selfTrans.position += transAmount;


            //var selfPos = selfTrans.position;
            //var targetPos = targetTrans.position;
            //var rate = BezierCurveCalculator.CalculateBezierPointX(journeyTime / timeScale, controlPoints);
            //selfTrans.position = Vector3.Lerp(startPos, targetPos, rate);

            //if (selfPos == targetPos)
            //{
            //    selfTrans.position = targetPos;
            //    break;
            //}
            yield return new WaitForFixedUpdate();
        }
    }


    /// <summary>
    /// 某方向位移动画任务: 指定自身trans属性, 位移方向, 位移距离, 动画时长, 动画速度效果(值曲线模式)
    /// </summary>
    /// <param name="selfTrans"></param>
    /// <param name="targetTrans"></param>
    /// <param name="journeyDuration"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public IEnumerator TranslateAnimTask(Transform selfTrans, Vector3 direction, float transitionDistance, float journeyDuration, Mode mode = Mode.LINEAR)
    {
        if (!Effects.ContainsKey(mode)) yield return null;
        var controlPoints = Effects[mode];
        double normalizedTime = 0;
        var oldAccelerationTimeRate = Vector3.zero;
        var resolution = 500;  //采样率
        double startTime = Time.time;
        float accelerationCurveScale = CalculateBezierCurveScale(controlPoints, transitionDistance, resolution);
        var transitionTime = resolution / journeyDuration;
        float deltatime = 1f / resolution;
        double planStepLength = 1f / resolution / journeyDuration;
        double lastTime = 0f;
        double time = 0;
        Debug.Log(startTime);
        while (true) 
        { 
            //Debug.Log($"stepLength: {stepLength}");
            time += planStepLength;
            var currentTime = time;

            double stepLength = currentTime - lastTime;
            lastTime = currentTime;
            var normalizedStepLength = stepLength;
            normalizedTime += normalizedStepLength;
            if (normalizedTime > 1)
                normalizedTime = 1;

            var currentAccelerationTimeRate = BezierCurveCalculator.CalculateBezierPoint((float)normalizedTime, controlPoints);
            float k = CalculatePointsK(currentAccelerationTimeRate, oldAccelerationTimeRate);
            oldAccelerationTimeRate = currentAccelerationTimeRate;

            var acceleration = k * accelerationCurveScale / journeyDuration;// * transitionDistance * normalizedStepLength * transitionTimes;
            var transAmount = direction * (float)acceleration;
            selfTrans.position += transAmount;
            //Debug.Log($"normalizedTime: {normalizedTime}, acceleration: {acceleration}, selfTrans.position: {selfTrans.position}");

            if (normalizedTime >= 1)
            {
                break;
            }
            yield return new WaitForSecondsRealtime(deltatime);
        }
        var endTime = Time.time;
        //Debug.Log($"endTime: {endTime}, continuousTime: {endTime-startTime}");
    }


    private float CalculatePointsK(Vector3 currentAccelerationTimeRate, Vector3 oldAccelerationTimeRate)
    {
        var accelerationTimeRate = currentAccelerationTimeRate - oldAccelerationTimeRate;
        var k = accelerationTimeRate.x == 0 ? 0 : accelerationTimeRate.y / accelerationTimeRate.x;
        return k;
    }


    private float CalculateBezierCurveTransAmount(Vector3[] points, float resolution)
    {
        float totalTransAmount = 0;
        var previousPoint = points[0];
        for (int i = 1; i <= resolution; i++)
        {
            var t = i / resolution;
            var currentPoint = BezierCurveCalculator.CalculateBezierPoint(t, points);
            totalTransAmount += CalculatePointsK(currentPoint, previousPoint);
            previousPoint = currentPoint;
        }
        return totalTransAmount;
    }


    private float CalculateBezierCurveScale(Vector3[] points, float distance, float resolution)
    {
        var totalTransAmount = CalculateBezierCurveTransAmount(points, resolution);
        var scale = distance / totalTransAmount;
        return scale;
    }


}
