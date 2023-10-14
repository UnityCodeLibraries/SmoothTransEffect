using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 向目标移动并检测是否越过目标<para/>
/// 配置: orgin(0, 0, 0), target(3, 0, 0)<para/>
/// 运行: <para/>
/// - 源向目标迭代位移x2, 并检测是否越过: 没有则继续, 否则停止并输出<para/>
/// 遇到的问题: <para/>
/// - 程序并没有在即将越过时检测并停止, 而是已经越过后, 在下一次位移时才发现越过目标; 这是由于以当前位置做为判断依据, 所以以运动后位置laterPos作为依据再次尝试 -- √验证确认
/// </summary>
public class TestVectorDirDetection : MonoBehaviour
{
    public GameObject Origin;
    public GameObject Target;
    public bool StartToggle;
    public bool MovingToggle;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (StartToggle)
        {
            StartToggle = false;
            MovingToggle = true;

            MoveAndDetection();
        }
    }


    Vector3 lastDir;
    private void MoveAndDetection()
    {
        if (!MovingToggle) return;

        var selfPos = Origin.transform.position;
        var targetPos = Target.transform.position;
        var laterPos = selfPos + new Vector3(2, 0, 0);

        //判断方向
        var targetDir = targetPos - laterPos;
        var areOpposite = Vector3.Dot(lastDir, targetDir);
        if (areOpposite < 0)
        {
            Debug.Log("方向相反");
            MovingToggle = false;
            return;
        }
        //更新此次运动方向
        lastDir = targetDir;

        //移动
        Debug.Log("Move add x2.");
        Origin.transform.position = laterPos;
    }


}
