using Cysharp.Threading.Tasks;
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
            if(currentGrouthCount < 0 || currentGrouthCount >= growthScale.Length) return;
            transform.localScale = new Vector3(growthScale[currentGrouthCount - 1], 5, growthScale[currentGrouthCount - 1]);
        }
        public override void OnMature()
        {
            transform.localScale = new Vector3(1f, 5f, 1f);
        }

        public void OnFallCollision(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlantBase>(out var plant)
               && plant.plantType == PlantType.Carrot) { 

            }
        }
        public override void OnEat()
        {
            BuffBase buffBase = PlayerController.Instance.transform.TryAddComponent<BigModelBuff>();
            BuffManager.Instance.AddBuff(PlayerController.Instance.transform, buffBase );
        }
    }
}