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
            Debug.Log("EndBuff");
            Destroy(this);
           
        }
        protected virtual void OnBuffUpdate(float remainingTime)
        {
            OnBuffUpdateCallback?.Invoke(remainingTime);
        }

        protected PlayerController GetBuffTarget()
        {
            if (this == null) {
                return null;
            }
            return transform.GetComponent<PlayerController>();
        }
    }
}