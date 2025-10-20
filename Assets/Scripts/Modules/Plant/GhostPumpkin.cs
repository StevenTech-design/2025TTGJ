using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using Unity.VisualScripting;
using UnityEngine;

namespace TTGJ.Plant
{
    public class GhostPumpkin : PlantBase
    {
        [SerializeField]
        private float intervalTimer = 0.5f;
        private float currentTime;
        private GameObject currentEatObject;
        

        private void Flashing() { 
            transform.DOScale(Vector3.one * 1.5f, 1).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        public override void OnEat()
        {
            base.OnEat();
            HeadFlashBuff flashBuff = BuffManager.Instance.AddBuff<HeadFlashBuff>(PlayerController.Instance.transform,false);
            ReplaceHeadBuff replaceHeadBuff = BuffManager.Instance.AddBuff<ReplaceHeadBuff>(PlayerController.Instance.transform,false);
            RevealBuff revealBuff = BuffManager.Instance.AddBuff<RevealBuff>(PlayerController.Instance.transform,false);
            flashBuff.StartBuff();
            replaceHeadBuff.StartBuff();
            revealBuff.StartBuff();
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
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
        }
        private async Task OnTriggerEnter(Collider other) {
            if (currentState != PlantState.Harvest ||other.gameObject.CompareTag("Field") || other.gameObject.layer == LayerMask.NameToLayer("Building") || currentEatObject != null) { 
                return;
            }
            currentEatObject = other.gameObject;
            other.gameObject.SetActive(false);
            await UniTask.Delay(1000);
            other.gameObject.SetActive(true);
            if(other.gameObject.TryGetComponent<Rigidbody>(out var rigidbody)) {
                rigidbody.AddForce(Vector3.left * 10, ForceMode.Impulse);
            }
            currentEatObject = null;
        }
    }
}