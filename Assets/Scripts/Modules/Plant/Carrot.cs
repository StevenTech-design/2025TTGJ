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

        public void OnFallCollision(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlantBase>(out var plant)
               && plant.itemType == ItemType.Carrot) { 

            }
        }
        public override void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<BigModelBuff>(PlayerController.Instance.transform);
            buffBase.StartBuff();
        }
    }
}