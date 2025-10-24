using TTGJ.GamePlay;
using TTGJ.Buff;
using TTGJ.Generate;
using UnityEngine;

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

        private void PlaySound() { 
            Debug.Log("PlaySound");
        }
        protected override void OnHarvest() {
            collider.enabled = true;
            collider.isTrigger = false;
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
    }
}