using Cysharp.Threading.Tasks;
using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Potato : PlantBase
    {
        [Tooltip("unit: second")]
        [SerializeField]
        private float matureBoomTime = 10;
        private float matureTime;
        private bool isNormalHarvest = true;

        public override void OnMature()
        {
            matureTime = Time.time;
            base.OnMature();
        }

        private void OnTriggerStay(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            if (Time.time - matureTime >= matureBoomTime)
            {
                //TODO: Create 5 havest potato
                isNormalHarvest = false;
                ChangeState(PlantState.Harvest);

            }
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            _ = SpawnMorePotato(isNormalHarvest ? 2 : 5);
            FallCollisionBuff fallCollisionBuff = BuffManager.Instance.AddBuff<FallCollisionBuff>(transform);
            fallCollisionBuff.OnCollisionEnterCallback += OnFallCollision;
            fallCollisionBuff.StartBuff();
        }

        public void OnFallCollision(Collision collision)
        {
            Debug.Log("Potato OnFallCollision play pen pen pen");
        }
        public override void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<FastMoveBuff>(PlayerController.Instance.transform);
            buffBase.StartBuff();
        }
        private async UniTask SpawnMorePotato(int count)
        {
            transform.GetComponent<Rigidbody>().AddForce(Vector3.left * 1, ForceMode.Impulse);
            for (int i = 0; i < count-1; i++)
            {
            GameObject potato = GameObject.Instantiate(gameObject, transform.position + Vector3.up * collider.bounds.size.y, transform.rotation);
            potato.GetComponent<Potato>().currentState = PlantState.Harvest;
            potato.GetComponent<Potato>().InitModel();
            potato.transform.localScale = transform.localScale;
            transform.GetComponent<Rigidbody>().AddForce(Vector3.left * 1, ForceMode.Impulse);
            await UniTask.Delay(100);
            }
        }

    }
}