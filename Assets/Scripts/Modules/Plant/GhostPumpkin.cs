using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Plant
{
    public class GhostPumpkin : PlantBase
    {
        [SerializeField]
        private float intervalTimer = 0.5f;
        private float currentTime;
        [SerializeField]
        private float bounceForce = 10;
        private void OnTriggerEnter(Collider other) {
            if (currentState != PlantState.Mature)
            {
                return;
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
            { 
                return;
            }
            
            currentTime = Time.time;
        }
        private void OnTriggerStay(Collider other) {
            if (currentState != PlantState.Mature)
            {
                return;
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Building") || other.CompareTag("Field"))
            { 
                return;
            }
            if (currentState != PlantState.Mature)
            {
                return;
            }
            if (Time.time - currentTime >= intervalTimer) {
                SpecialAction(new PlantSpacialParam<GameObject> { param = other.gameObject });
                currentTime = Time.time;
            }
        }
        public override void SpecialAction(PlantSpacialParam param) { 
            if (!(param is PlantSpacialParam<GameObject> plantSpacialParam))
            {
                return;
            }
            plantSpacialParam.param.GetComponent<Rigidbody>().AddForce(Vector3.left * bounceForce, ForceMode.Impulse);
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
            FallCollisionBuff fallCollisionBuff = transform.TryAddComponent<FallCollisionBuff>();
            fallCollisionBuff.OnCollisionEnterCallback += OnFallCollision;
            BuffManager.Instance.AddBuff(transform, fallCollisionBuff);
        }
        public async void OnFallCollision(Collision collision)
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player)) {
                Camera.main.gameObject.SetActive(false);
                await UniTask.Delay(1000);
                Camera.main.gameObject.SetActive(true);
            }else {
                if (collision.gameObject.CompareTag("Field")) {
                    return;
                }
                collision.gameObject.SetActive(false);
                await UniTask.Delay(1000);
                collision.gameObject.SetActive(true);
            }
        }
    }
}