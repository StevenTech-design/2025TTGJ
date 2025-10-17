using System.Collections.Generic;
using TTGJ.Plant;

namespace TTGJ.Buff
{
    public class WateringBuff : BuffBase
    {
        protected override void StartBuff()
        {
            base.StartBuff();
            Watering();
            EndBuff();
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
    }
}