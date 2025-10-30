using System;
using DG.Tweening;
using TTGJ.Audio;
using TTGJ.Buff;
using TTGJ.Config;
using TTGJ.GamePlay;
using TTGJ.House;
using UnityEngine;

namespace TTGJ.Plant {
    public class Grass : PlantBase
    {
        public Action<GameObject, int> OnGrassHarvested;
        public int index;
        public override void OnMature()
        {

        }
        public override void OnGermination()
        {

        }
        public override void OnWatering()
        {
            ++currentGrouthCount;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f));
            mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f).SetEase(Ease.OutElastic));
        }
        protected override void OnHarvest()
        {
            currentState = PlantState.Harvest;
            transform.DOKill();
            transform.DOScale(Vector3.one * ConfigManager.Instance.GetHaverstSize(itemType, currentGrouthCount), 1f);
            OnGrassHarvested?.Invoke(gameObject, index);
            HouseManager.Instance.CollectPlant(this);
        }
        public override void OnEat()
        {
            SheepTalkBuff buff = BuffManager.Instance.AddBuff<SheepTalkBuff>(PlayerController.Instance.transform);
            buff.StartBuff();
        }
        private void OnTriggerEnter(Collider other)
        {
            AudioManager.Instance.PlaySFX(Audio.AudioType.Bush_Swaying);
        }
    }
}
