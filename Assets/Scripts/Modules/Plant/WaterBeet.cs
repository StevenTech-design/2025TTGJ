using System.Collections.Generic;
using TTGJ.Common;
using UnityEngine;
using TTGJ.Buff;
using Unity.VisualScripting;
using TTGJ.Config;

namespace TTGJ.Plant
{
    public class WaterBeet : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if(currentState < PlantState.Mature) { 
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
            int currentOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount);
            List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos, currentOccupiedFieldSize);
            Debug.Log("WateringOther: " + plants.Count);
            foreach (var plant in plants)
            {
                plant.OnWatering();
            }
        }
        private void OnCollisionStay(Collision collision)
        {
            if(currentState <= PlantState.Mature) { 
                return;
            }
            if (collision.transform.TryGetComponent<Cell>(out var cell))
            {
                WateringOther();
                Destroy(gameObject);
            }
        }
        
    }
}