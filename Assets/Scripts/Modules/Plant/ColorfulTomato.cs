using System.Collections.Generic;
using TTGJ.Config;
using UnityEngine;

namespace TTGJ.Plant
{
    public class ColorfulTomato : PlantBase
    {
            [SerializeField]
        private int colorID;
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            int currentOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount);
             List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos,currentOccupiedFieldSize);
             foreach (var plant in plants)
             {
                plant.OnWatering();
                plant.Dyeing(colorID);
             }
        }
    }
}