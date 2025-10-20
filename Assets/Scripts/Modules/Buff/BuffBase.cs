using System;
using TTGJ.Framework.Timer;
using TTGJ.GamePlay;
using UnityEngine;
namespace TTGJ.Buff
{
    public class BuffBase : MonoBehaviour
    {
        public float duration = 30;
        private int timerTaskId;
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
            if (transform == null) {
                return;
            }
            OnBuffEndCallback?.Invoke();
            TimerManager.Instance.StopTimer(timerTaskId);
            BuffManager.Instance.RemoveBuff(transform, this);
            Destroy(GetComponent<BuffBase>());
        }
        protected virtual void OnBuffUpdate(float remainingTime)
        {
            OnBuffUpdateCallback?.Invoke(remainingTime);
        }

        protected PlayerController GetBuffTarget()
        {
            if (transform == null) {
                return null;
            }
            return transform.GetComponent<PlayerController>();
        }
    }
}