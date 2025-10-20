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
        private

        protected void OnMature()
        {
            base.OnMature();
            matureTime = Time.time;
        }

        private void OnTriggerStay(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            if (Time.time - matureTime >= matureBoomTime)
            {
                //TODO: Create 5 havest potato
                for (int i = 0; i < 5; i++)
                {
                    GameObject potato = GameObject.Instantiate(gameObject, transform.position + Vector3.up * collider.bounds.size.y * i, transform.rotation);
                    potato.GetComponent<Potato>().ChangeState(PlantState.Harvest);
                }
            }
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            GameObject potato = GameObject.Instantiate(gameObject, transform.position + Vector3.up * collider.bounds.size.y, transform.rotation);
            potato.GetComponent<Potato>().ChangeState(PlantState.Harvest);
            potato.GetComponent<Potato>().OnTeleport(baseTeleportPos);
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
    }
}