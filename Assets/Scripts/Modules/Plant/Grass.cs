using System;
using TTGJ.Buff;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Plant {
    public class Grass : PlantBase {
        public Action <int> OnGrassHarvested;
        public int index;
        private bool isHarvested = false;
        public override void OnMature() { 

        }
        public override void OnGermination() { 

        }
        public override void OnWatering() { 

        }
        public override void OnLift() { 
            base.OnLift();
            if(isHarvested) { 
                return;
            }
            isHarvested = true;
            Debug.Log("Grass OnHarvest: " + index);
            OnGrassHarvested?.Invoke(index);
        }
        public override void OnEat() { 
            SheepTalkBuff buff = BuffManager.Instance.AddBuff<SheepTalkBuff>(PlayerController.Instance.transform);
            buff.StartBuff();
        }
    }
}
