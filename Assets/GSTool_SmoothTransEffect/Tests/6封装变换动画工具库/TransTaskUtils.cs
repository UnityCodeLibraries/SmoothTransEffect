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
        PINGPONG1, EASE, EASE_IN, EASE_IN_OUT
    }

    /// <summary>
    /// 预设效果BezierPoints
    /// </summary>
    public static IDictionary<Mode, Vector3[]> Effects = new Dictionary<Mode, Vector3[]>()
    {
        { Mode.PINGPONG1, new Vector3[]{
            new Vector3(0, 0), 
            new Vector3(0f, .231f), 
            new Vector3(.288f, .925f), 
            new Vector3(1.212f, .517f), 
            new Vector3(.995f, .038f), 
            new Vector3(1, 1)
        } }, 
        { Mode.EASE, new Vector3[]{
            new Vector3(0, 1), 
            new Vector3(.141f, .755f), 
            new Vector3(1, .806f), 
            new Vector3(1, 0)
        } }, 
        { Mode.EASE_IN, new Vector3[]{
            new Vector3(0, 1), 
            new Vector3(0, .7f), 
            new Vector3(0, .7f), 
            new Vector3(1, 0)
        } }, 
        //{ Mode.EASE_IN_OUT, new Vector3[]{
        //    new Vector3(0, 1), 
        //    new Vector3(-0.004f, -1.11f), 
        //    new Vector3(0.351f, 0.198f), 
        //    new Vector3(1.307f, 1.337f), 
        //    new Vector3(1, 2.708f),
        //    new Vector3(1, 0),
        //} }, 
        { Mode.EASE_IN_OUT, LoadEffectData("Assets/GSTool_SmoothTransEffect/Tests/6封装变换动画工具库/EASE_IN_OUT.asset") },
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
    public IEnumerator TranslationTask(Transform selfTrans, Transform targetTrans, float timeScale, Mode mode = Mode.PINGPONG1)
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
    public IEnumerator TranslationTask2(Transform selfTrans, Transform targetTrans, float journeyDuration, Mode mode = Mode.PINGPONG1)
    {
        if (!Effects.ContainsKey(mode)) yield return null;
        var controlPoints = Effects[mode];
        var startTime = Time.time;
        var startPos = selfTrans.position;
        var startVel = 0f;
        var currentVel = 0f;
        var velMultiple = 3f;
        var oldDir = Vector3.zero;
        var oldJourneyTime = 0f;
        while (true)
        {
            var stepLength = Time.fixedDeltaTime / journeyDuration;
            //Debug.Log($"stepLength: {stepLength}");

            var currentTime = Time.time;
            var journeyTime = currentTime - startTime;
            if (journeyTime > journeyDuration) 
                journeyTime = journeyDuration;

            var oldAccelerationTimeRate = oldJourneyTime == 0 ? 0 : BezierCurveCalculator.CalculateBezierPointX(oldJourneyTime / journeyDuration, controlPoints);
            var currentAccelerationTimeRate = BezierCurveCalculator.CalculateBezierPointX(journeyTime / journeyDuration, controlPoints);
            var accelerationTimeRate = currentAccelerationTimeRate - oldAccelerationTimeRate;
            var planAccelerationTimeRate = journeyTime - oldJourneyTime;
            oldJourneyTime = journeyTime;

            var finalAccelerationTimeRate = planAccelerationTimeRate == 0?0:
                (accelerationTimeRate * 2 / planAccelerationTimeRate);
            var acceleration = finalAccelerationTimeRate * velMultiple;

            //Debug.Log($"finalAccelerationTimeRate: {finalAccelerationTimeRate}");

            var selfPos = selfTrans.position;
            var targetPos = targetTrans.position;
            var currentDir = (targetPos - selfPos).normalized;

            var transAmount = acceleration * currentDir;
            var laterPos = selfPos + transAmount;
            var laterDir = (targetPos - laterPos).normalized;

            if (Vector3.Dot(currentDir, laterDir) < 0)
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


}
