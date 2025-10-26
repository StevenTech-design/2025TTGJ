using System.Collections.Generic;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.Buff
{
    public class FieldDyeingBuff : BuffBase
    {
        private int colorID = 0;
        public override void StartBuff()
        {
            base.StartBuff();
            Dyeing();
        }
        private void Dyeing()
        {
            if (GetComponent<Cell>() == null) return;
            List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos, (int)transform.localScale.x);
            foreach (var plant in plants)
            {
                plant.Dyeing(colorID);
            }
        }
    }
}