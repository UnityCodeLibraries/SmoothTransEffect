using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ŀ���ƶ�������Ƿ�Խ��Ŀ��<para/>
/// ����: orgin(0, 0, 0), target(3, 0, 0)<para/>
/// ����: <para/>
/// - Դ��Ŀ�����λ��x2, ������Ƿ�Խ��: û�������, ����ֹͣ�����<para/>
/// ����������: <para/>
/// - ����û���ڼ���Խ��ʱ��Ⲣֹͣ, �����Ѿ�Խ����, ����һ��λ��ʱ�ŷ���Խ��Ŀ��; ���������Ե�ǰλ����Ϊ�ж�����, �������˶���λ��laterPos��Ϊ�����ٴγ��� -- ����֤ȷ��
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

        //�жϷ���
        var targetDir = targetPos - laterPos;
        var areOpposite = Vector3.Dot(lastDir, targetDir);
        if (areOpposite < 0)
        {
            Debug.Log("�����෴");
            MovingToggle = false;
            return;
        }
        //���´˴��˶�����
        lastDir = targetDir;

        //�ƶ�
        Debug.Log("Move add x2.");
        Origin.transform.position = laterPos;
    }


}
