using System;
using System.Collections;
using UnityEngine;

namespace GoldSprite.GSTools.BezierCurveAnim
{
    /// <summary>
    /// 使用变换动画封装库 <para/>
    /// </summary>
    public class TestTranslateAnimTask : MonoBehaviour
    {
        [Header("引用")]
        public Transform Target;
        public Transform[] Origins;
        public bool start;
        public float journeyDuration = 1.5f;
        public float distance = 10f;
        public Delegate[] deles = new Delegate[3];

        public TransTaskUtils utils;


        private void Start()
        {
            utils = new TransTaskUtils();
            Target = transform.GetChild(0);
        }


        private void Update()
        {
            if (start)
            {
                start = false;
                StartTransTask();
            }
        }


        private void StartTransTask()
        {
            var modes = new TransTaskUtils.Mode[] {
            TransTaskUtils.Mode.LINEAR,
            TransTaskUtils.Mode.EASE,
            TransTaskUtils.Mode.EASE_IN,
            };
            for (int i = 0; i < Origins.Length; i++)
            {
                var selfTrans = Origins[i];
                if (!selfTrans.gameObject.activeSelf) continue;
                var mode = modes[i];
                var targetTrans = Target;
                var timeScale = this.journeyDuration;
                var dir = new Vector3(1, 0, 0);
                StartCoroutine(utils.TranslateAnimTask(selfTrans, dir, distance, timeScale, mode));
            }
        }


    }


}