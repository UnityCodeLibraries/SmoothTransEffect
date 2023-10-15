using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 根据距离的加速度与减速度: <para/>
/// • 在距离一小半之前加速, 在距离大半之后减速<para/>
/// 不过最终效果还是不如意, 我也许该尝试手动绘制贝塞尔曲率<para/>
/// </summary>

public class TestContinuousTask_2 : MonoBehaviour
{
    [Header("引用")]
    public GameObject Origin;
    public GameObject Target;

    [Header("实时")]
    public bool StartToggle;
    public bool MovingToggle;
    [Range(0, 100)]
    public int MovingProcessNormalizedPercent;  //归一化距离进度百分比

    //配置
    public float TransTimeRate = 10f;  //预计3秒(目标位置不变)
    public float smoothDuration = 0.5f;  //过渡速度持续时间
#pragma warning disable IDE0044 // 添加只读修饰符
    public float smoothPoint = 2/5f;  //起速降速点
#pragma warning restore IDE0044 // 添加只读修饰符
    public float minVelRate = 0.08f;  //最低接近速度比率

    float totalDistance;
    float processNormalized;  //归一化距离进度
    float accelerationTime, decelerationTime, journeyTime;
    float timeRate;  //速度控制率


    // Start is called before the first frame update
    void Start()
    {
        Origin = gameObject;
        Target = GameObject.Find("Sphere2");
    }

    // Update is called once per frame
    void Update()
    {
        if (StartToggle)
        {
            StartToggle = false;
            if (!MovingToggle)
            {
                MovingToggle = true;
                StartCoroutine(MoveTask());
            }
        }
    }


    public IEnumerator MoveTask()
    {
        //开始前配置
        var selfPos = Origin.transform.position;
        var targetPos = Target.transform.position;
        totalDistance = Vector3.Distance(targetPos, selfPos);
        accelerationTime = Time.time;

        while (MovingToggle)
        {
            //计算进度
            selfPos = Origin.transform.position;
            targetPos = Target.transform.position;
            var currentDistance = Vector3.Distance(targetPos, selfPos);
            processNormalized = (totalDistance - currentDistance) / totalDistance;
            MovingProcessNormalizedPercent = (int)Mathf.Round(processNormalized * 100);

            //计算速度比距离乘率

            var realVelRate = totalDistance / 10f;  //距离10为单元距离
            var realMinVelRate = minVelRate * realVelRate;
            var realMaxVelRate = 1 * realVelRate;
            //计算速度平滑
            journeyTime = Time.time - accelerationTime;
            var journeyNormalizedTime = journeyTime / smoothDuration;
            var rate = realMaxVelRate;
            if (processNormalized < smoothPoint)
            {
                rate = processNormalized * (1/smoothPoint);
                timeRate = Mathf.Lerp(realMinVelRate, realMaxVelRate, rate);
            }
            else if (processNormalized > 1- smoothPoint)
            {
                rate = (processNormalized - (1 - smoothPoint)) * (1 / smoothPoint);
                timeRate = Mathf.Lerp(realMaxVelRate, realMinVelRate, rate);
            }
            else 
                timeRate = rate;
            Debug.Log($"timeRate: {timeRate}, realMinVelRate: {realMinVelRate}");

            MoveAndDetection();

            yield return new WaitForFixedUpdate();
        }
    }


    Vector3 lastDir;
    private void MoveAndDetection()
    {
        if (!MovingToggle) return;

        //配置
        var moveRate = 1 / 60f * TransTimeRate;
        var movingInterpolation = moveRate * timeRate;
        //movingInterpolation = moveRate * 0;
        //movingInterpolation = moveRate * 1;
        //movingInterpolation = moveRate / moveRate;

        var selfPos = Origin.transform.position;
        var targetPos = Target.transform.position;
        var laterPos = Vector3.MoveTowards(selfPos, targetPos, movingInterpolation);

        //判断方向
        var targetDir = targetPos - laterPos;
        var areOpposite = Vector3.Dot(lastDir, targetDir);
        if(areOpposite < 0) Debug.Log("方向相反");

        if (areOpposite < 0 || processNormalized >= 1)
        {
            Debug.Log("位移任务结束");
            MovingToggle = false;
            laterPos = targetPos;
        }
        else
        {
            //更新此次运动方向
            lastDir = targetDir;
        }

        //移动
        if (!MovingToggle) Debug.Log($"Move arrive.");
        else Debug.Log($"Move add x{movingInterpolation}.");
        Origin.transform.position = laterPos;

    }


}
