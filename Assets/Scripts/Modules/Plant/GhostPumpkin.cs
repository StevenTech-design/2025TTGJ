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
            List<BuffBase> buffList = new List<BuffBase>();
            BuffBase flashBuff = PlayerController.Instance.transform.TryAddComponent<HeadFlashBuff>();
            BuffBase replaceHeadBuff = PlayerController.Instance.transform.TryAddComponent<ReplaceHeadBuff>();
            BuffBase revealBuff = PlayerController.Instance.transform.TryAddComponent<RevealBuff>();
            buffList.Add(flashBuff);
            buffList.Add(replaceHeadBuff);
            buffList.Add(revealBuff);
            BuffManager.Instance.AddBuff(PlayerController.Instance.transform, buffList);
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            // FallCollisionBuff fallCollisionBuff = transform.TryAddComponent<FallCollisionBuff>();
            // fallCollisionBuff.OnCollisionEnterCallback += OnFallCollision;
            // BuffManager.Instance.AddBuff(transform, fallCollisionBuff);
            collider.isTrigger = true;
        }
        public override void OnMature()
        {
            base.OnMature();
            Flashing();
        }
        private async Task OnTriggerEnter(Collider other) {
            if (other.gameObject.CompareTag("Field") || other.gameObject.layer == LayerMask.NameToLayer("Building") || currentEatObject != null) { 
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
        // public async void OnFallCollision(Collision collision)
        // {
        //     if (collision.gameObject.TryGetComponent<PlayerController>(out var player)) {
        //         Camera.main.gameObject.SetActive(false);
        //         await UniTask.Delay(1000);
        //         Camera.main.gameObject.SetActive(true);
        //     }else {
        //         if (collision.gameObject.CompareTag("Field")) {
        //             return;
        //         }
        //         collision.gameObject.SetActive(false);
        //         await UniTask.Delay(1000);
        //         collision.gameObject.SetActive(true);
        //     }
        // }
    }
}