using System.Collections.Generic;
using TTGJ.Config;
using TTGJ.Interactable;
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
        private void OnCollisionEnter(Collision collision) {
            if (collision.collider.gameObject.TryGetComponent<IDyeingable>(out var dye)) { 
                Debug.Log("ColorfulTomato OnCollisionEnter: " + collision.collider.gameObject.name);
                dye.Dyeing(colorID);
            }
        }

    }
}