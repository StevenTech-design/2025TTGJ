using System;
using TTGJ.Framework.Timer;
using TTGJ.GamePlay;
using UnityEngine;
namespace TTGJ.Buff
{
    public class BuffBase : MonoBehaviour
    {
        public float duration;
        public float checkInterval;
        private int timerTaskId;
        private float lastCheckTime;
        public Action OnBuffStartCallback;
        public Action<float> OnBuffUpdateCallback;
        public Action OnBuffEndCallback;


        public virtual void StartBuff()
        {
            timerTaskId = TimerManager.Instance.StartTimer(duration, EndBuff, OnBuffUpdate);
            OnBuffStartCallback?.Invoke();
        }
        public virtual void EndBuff()
        {
            if (transform == null)
            {
                return;
            }
            OnBuffEndCallback?.Invoke();
            TimerManager.Instance.StopTimer(timerTaskId);
            BuffManager.Instance.RemoveBuff(transform, this);
            Debug.Log("EndBuff");
            Destroy(this);

        }
        protected virtual void OnBuffUpdate(float remainingTime)
        {
            if (Time.time - lastCheckTime < checkInterval)
            {
                return;
            }
            lastCheckTime = Time.time;
            OnBuffUpdateCallback?.Invoke(remainingTime);
        }

        protected PlayerController GetBuffTarget()
        {
            if (this == null)
            {
                return null;
            }
            return transform.GetComponent<PlayerController>();
        }
        public virtual BuffType GetBuffType()
        {
            return BuffType.None;
        }
    }
}