using TTGJ.GamePlay;
using TTGJ.Buff;
using TTGJ.Generate;
using UnityEngine;
using DG.Tweening;
using TTGJ.Config;
using TTGJ.House;

namespace TTGJ.Plant
{
    public class KiwiFruit : PlantBase
    {
        [SerializeField]
        private float force = 1f;
        public override void OnEat() { 
            ReplaceModelBuff buffBase = BuffManager.Instance.AddBuff<ReplaceModelBuff>(PlayerController.Instance.transform);
            buffBase.pathModelPath = ResPathConfig.OriginModel_Kivi;
            buffBase.originalModel = PlayerController.Instance.modelTransform.gameObject;
            buffBase.StartBuff();
        }

        private void OnCollisionEnter(Collision other) { 
            if(other.gameObject.layer == LayerMask.NameToLayer("Building") || other.gameObject.CompareTag("Field")) { 
                return;
            }
            Vector3 direction = other.transform.position - transform.position;
            rigidbody.AddForce(direction.normalized * force, ForceMode.Impulse);
            PlaySound();
        }

        public override void OnWatering() { 
            ++currentGrouthCount;
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f));
            mySequence.Append(transform.DOScale(ConfigManager.Instance.GetPlantGrowthModelSize(itemType, currentGrouthCount), 0.5f).SetEase(Ease.OutElastic));
            mySequence.Play();
        }

        private void PlaySound() { 
            Debug.Log("PlaySound");
        }
        protected override void OnHarvest()
        {
            collider.enabled = true;
            collider.isTrigger = false;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
            HouseManager.Instance.CollectPlant(this);
        }
    }
}