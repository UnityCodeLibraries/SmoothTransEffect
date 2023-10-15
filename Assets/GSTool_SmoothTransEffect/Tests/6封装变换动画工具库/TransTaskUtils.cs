using System;
using System.Collections;
using System.Collections.Generic;
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
        { Mode.EASE_IN_OUT, new Vector3[]{
            new Vector3(0, 1), 
            new Vector3(0, .56f), 
            new Vector3(1, .44f), 
            new Vector3(1, 0)
        } }, 
    };


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
            var selfPos = selfTrans.position;
            var targetPos = targetTrans.position;
            var rate = BezierCurveCalculator.CalculateBezierPointX(journeyTime / timeScale, controlPoints);
            selfTrans.position = Vector3.Lerp(selfPos, targetPos, rate);

            if (selfPos == targetPos)
            {
                selfTrans.position = targetPos;
                break;
            }
            yield return new WaitForFixedUpdate();
        }
    }


}
