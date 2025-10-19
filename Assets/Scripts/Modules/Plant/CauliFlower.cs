using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Plant
{
    public class CauliFlower : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            Debug.Log("CauliFlower OnTriggerEnter  mei mei");
        }

        protected override void OnHarvest()
        {
            base.OnHarvest();
            Debug.Log("Become sheep");
        }

         
    }
}