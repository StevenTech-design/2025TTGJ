using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Plant
{
    public class ColorfulTomato : PlantBase
    {
        [SerializeField]
        private Color dyeingColor;
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
             List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos,growthScale[currentGrouthCount -1]);
             foreach (var plant in plants)
             {
                plant.OnWatering();
                plant.Dyeing(dyeingColor);
             }
        }
    }
}