using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.Plant
{
    public class CauliFlower : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("CauliFlower OnTriggerEnter  mei mei");
        }

        public override void OnHarvest()
        {
            base.OnHarvest();
            Debug.Log("Become sheep");
        }

         
    }
}