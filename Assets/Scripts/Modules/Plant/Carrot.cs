using Cysharp.Threading.Tasks;
using DG.Tweening;
using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Carrot : PlantBase
    {
        public async override UniTask OnWatering()
        {
            await base.OnWatering();
            if (currentGrouthCount < 0 || currentGrouthCount >= growthScale.Length) return;
            transform.DOScale(new Vector3(growthScale[currentGrouthCount - 1], 5, growthScale[currentGrouthCount - 1]),0.5f);
        }
        public override void OnMature()
        {
            transform.DOScaleY(5, 0.5f);
        }

        public void OnFallCollision(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlantBase>(out var plant)
               && plant.plantType == PlantType.Carrot) { 

            }
        }
        public override void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<BigModelBuff>(PlayerController.Instance.transform);
            buffBase.StartBuff();
        }
    }
}