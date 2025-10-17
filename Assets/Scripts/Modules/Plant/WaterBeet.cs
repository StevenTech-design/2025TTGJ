using System.Collections.Generic;
using UnityEngine;

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
    }
}