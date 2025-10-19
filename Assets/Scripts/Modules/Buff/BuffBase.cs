using TTGJ.Framework.Timer;
using TTGJ.GamePlay;
using UnityEngine;
namespace TTGJ.Buff
{
    public class BuffBase : MonoBehaviour
    {
        public float duration = 30;
        private int timerTaskId;


        public virtual void StartBuff()
        {
            timerTaskId = TimerManager.Instance.StartTimer(duration, EndBuff, OnBuffUpdate);
        }
        public virtual void EndBuff()
        {
            if (transform == null) {
                return;
            }
            TimerManager.Instance.StopTimer(timerTaskId);
            BuffManager.Instance.RemoveBuff(transform, this);
            Destroy(GetComponent<BuffBase>());
        }
        protected virtual void OnBuffUpdate(float remainingTime)
        {

        }

        protected PlayerController GetBuffTarget()
        {
            return transform.GetComponent<PlayerController>();
        }
    }
}