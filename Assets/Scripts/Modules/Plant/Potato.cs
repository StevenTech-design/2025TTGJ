using TTGJ.Plant;
using Unity.VisualScripting;
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
        public override void OnTeleport()
        {
            base.OnTeleport();
            GameObject potato = GameObject.Instantiate(gameObject, transform.position + Vector3.up * collider.bounds.size.y, transform.rotation);
            potato.GetComponent<Potato>().ChangeState(PlantState.Harvest);
            potato.GetComponent<Potato>().OnTeleport();
        }
        

        
    }
}