using System.Collections.Generic;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.Buff
{
    public class FieldWateringBuff : BuffBase
    {
        public override void StartBuff()
        {
            base.StartBuff();
        }
        private void Watering()
        {
            if (GetComponent<Cell>() == null) return;
            List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos, (int)transform.localScale.x);
            foreach (var plant in plants)
            {
                plant.OnWatering();
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.transform.TryGetComponent<Cell>(out var cell)) { 
                Watering();
                EndBuff();
            }
        }

    }
}