using System;
using DG.Tweening;
using Steven.Framework;
using UnityEngine;

namespace TTGJ.QTE
{
    public enum QTEType
    {
        Failure,
        Hit,
        Perfect,
    }
    public class QTESystem : MonoSingleton<QTESystem>
    {
        [SerializeField] private Vector2 perfectDirection;
        [SerializeField] private Vector2 perfectAngle;
        [SerializeField] private Vector2 hitAngle;
        [SerializeField] private float moveAngle = 90f;
        [SerializeField] private float cursorSpeed = 1f;
        private float currentAngle = 0;
        private bool isQTEStarted = false;
        private Tween angleTween;
        private Action<QTEType> onComplete;
        public void StartQTE(Action<QTEType> onComplete = null)
        {
            if (isQTEStarted)
            {
                Debug.LogWarning("QTESystem: QTE已在进行中");
                return;
            }
            isQTEStarted = true;
            currentAngle = 0f;
            this.onComplete = onComplete;
            angleTween = DOTween.To(
                () => currentAngle, 
                x => currentAngle = x,
                 moveAngle, cursorSpeed)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Yoyo)
                 .OnKill(()=>{
                    isQTEStarted = false;
                 });
        }
        public QTEType EndQTE()
        {
            angleTween.Kill();
            angleTween = null;
            QTEType result = GetQTEType();
            onComplete?.Invoke(result);
            onComplete = null;
            return result;
            
        }
        private QTEType GetQTEType() {
            if (currentAngle > perfectAngle.x && currentAngle < perfectAngle.y)
            {
                return QTEType.Perfect;
            }
            else if (currentAngle > hitAngle.x && currentAngle < hitAngle.y)
            {
                return QTEType.Hit;
            }
            else
            {
                return QTEType.Failure;
            }
        }

        // private void Update() {
        //     if(isQTEStarted) {
        //         currentAngle += Time.deltaTime * cursorSpeed;
        //     }
        // }
    }
}