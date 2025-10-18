using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Plant
{
    public class ColorfulTomato : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
             Debug.Log("ColorfulTomato OnTriggerEnter: "+other.gameObject.name + " to "+GetComponent<Cell>());
             List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos,growthScale[currentGrouthCount -1]);
             foreach (var plant in plants)
             {
                _ = plant.OnWatering();
                Debug.Log("ColorfulTomato OnTriggerEnter watering plant dyeing");
             }
        }
    }
}