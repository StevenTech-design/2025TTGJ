using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TTGJ.Audio;
using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Plant
{
    public class GhostPumpkin : PlantBase
    {
        private GameObject currentEatObject;

        [SerializeField] private float interval = 0.3f;
        [SerializeField] private float force = 10;
        private Tween blinkTween;

        

        void Toggle()
        { 
            modelRoot.gameObject.SetActive(!modelRoot.gameObject.activeSelf);
        }
        

        private void Flashing() { 
            blinkTween = DOVirtual.DelayedCall(interval, Toggle, false)
                .SetLoops(-1, LoopType.Restart);
        }

        public override void OnEat()
        {
            base.OnEat();
            RevealBuff revealBuff = BuffManager.Instance.AddBuff<RevealBuff>(PlayerController.Instance.transform,false);
            revealBuff.StartBuff();
            ReplaceHeadBuff headBuff = BuffManager.Instance.AddBuff<ReplaceHeadBuff>(PlayerController.Instance.headTransform,false);
            headBuff.modelPrefab = this.gameObject;
            headBuff.StartBuff();
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            blinkTween?.Kill();
            modelRoot.gameObject.SetActive(true);
        }
        public override void OnMature()
        {
            base.OnMature();
            Flashing();
        }
        protected override void OnHarvest()
        {
            base.OnHarvest();
            transform.DOKill();
            modelRoot.gameObject.SetActive(true);
        }
        private async void OnCollisionEnter(Collision collision) {
            GameObject other = collision.collider.gameObject;
            if (currentState != PlantState.Harvest 
            || other.CompareTag("Field") 
            || other.layer == LayerMask.NameToLayer("Building") 
            || currentEatObject != null) { 
                return;
            }
            Debug.Log("OnCollisionEnter111: " + other.name);
            currentEatObject = other;
            GetComponentInChildren<Animator>().SetTrigger("Eat");
            other.SetActive(false);
            AudioManager.Instance.PlaySFX(Audio.AudioType.Pumpkin_Swallowing);
            await UniTask.Delay(1000);
            other.SetActive(true);
            if(other.TryGetComponent<Rigidbody>(out var rigidbody)) {
                other.transform.position = transform.position + Vector3.left * 2;
                rigidbody.AddForce(force * Time.deltaTime * Vector3.left, ForceMode.Impulse);
            }
            currentEatObject = null;
        }
    }
}