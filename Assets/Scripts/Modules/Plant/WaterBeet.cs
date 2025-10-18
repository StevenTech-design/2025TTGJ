using System.Collections.Generic;
using TTGJ.Common;
using UnityEngine;
using TTGJ.Buff;

namespace TTGJ.Plant
{
    public class WaterBeet : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos, growthScale[currentGrouthCount - 1]);
            foreach (var plant in plants)
            {
                plant.OnWatering();
            }
        }
        public override void OnTeleport(Transform baseTeleportPos) { 
            base.OnTeleport(baseTeleportPos);
            BuffManager.Instance.AddBuff(transform,transform.TryAddComponent<FieldWateringBuff>());
        }
    }
}