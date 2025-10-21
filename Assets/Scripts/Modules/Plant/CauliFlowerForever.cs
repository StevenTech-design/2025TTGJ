using System.Collections.Generic;
using TTGJ.Buff;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Plant
{
    public class CauliFlowerForever : PlantBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            BuffBase buffBase = BuffManager.Instance.AddBuff<SmallModelBuff>(PlayerController.Instance.transform);
            buffBase.StartBuff();
        }
        public override void OnEat() { 
            BuffBase buffBase = BuffManager.Instance.AddBuff<SmallModelBuff>(PlayerController.Instance.transform);
            buffBase.StartBuff();
        }
        protected override void OnHarvest()
        {
            base.OnHarvest();
            Debug.Log("Become sheep");
        }

         
    }
}