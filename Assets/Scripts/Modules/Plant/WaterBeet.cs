using System.Collections.Generic;
using TTGJ.Common;
using UnityEngine;
using TTGJ.Buff;
using Unity.VisualScripting;

namespace TTGJ.Plant
{
    public class WaterBeet : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            WateringOther();
        }
        public override void OnTeleport(Transform baseTeleportPos) { 
            base.OnTeleport(baseTeleportPos);
            BuffBase buffBase = BuffManager.Instance.AddBuff<FieldWateringBuff>(transform);
            buffBase.StartBuff();
        }
        private void WateringOther()
        { 
            List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos, growthScale[currentGrouthCount - 1]);
            foreach (var plant in plants)
            {
                plant.OnWatering();
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if (collision.transform.TryGetComponent<Cell>(out var cell))
            {
                WateringOther();
                Destroy(gameObject);
            }
        }
        
    }
}