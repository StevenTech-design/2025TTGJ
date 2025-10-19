using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Plant
{
    public class ColorfulTomato : PlantBase
    {
        [SerializeField]
        private Color dyeingColor;
        private async void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            Debug.Log("ColorfulTomato OnTriggerEnter watering plant");
             List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos,growthScale[currentGrouthCount -1]);
             foreach (var plant in plants)
             {
                await plant.OnWatering();
                plant.Dyeing(dyeingColor);
                Debug.Log("ColorfulTomato OnTriggerEnter watering plant dyeing");
             }
        }
    }
}