using System;
using DG.Tweening;
using TTGJ.Buff;
using TTGJ.Config;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Plant {
    public class Grass : PlantBase {
        public Action <GameObject,int> OnGrassHarvested;
        public int index;
        public override void OnMature() { 

        }
        public override void OnGermination() { 

        }
        public override void OnWatering() {
            ++currentGrouthCount;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f));
            mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f).SetEase(Ease.OutElastic));
        }
        protected override void OnHarvest() { 
            currentState = PlantState.Harvest;
            OnGrassHarvested?.Invoke(gameObject,index);
        }
        public override void OnEat() { 
            SheepTalkBuff buff = BuffManager.Instance.AddBuff<SheepTalkBuff>(PlayerController.Instance.transform);
            buff.StartBuff();
        }
    }
}
