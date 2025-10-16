using TTGJ.Framework.Timer;
using TTGJ.GamePlay;
using UnityEngine;
namespace TTGJ.Buff
{
    public class BuffBase : MonoBehaviour
    {
        public float duration = 30;
        private int timerTaskId;


        protected virtual void StartBuff()
        {
            timerTaskId = TimerManager.Instance.StartTimer(duration, EndBuff, OnBuffUpdate);
        }
        protected virtual void EndBuff()
        {
            TimerManager.Instance.StopTimer(timerTaskId);
            BuffManager.Instance.RemoveBuff(transform, this);
            Destroy(this);
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