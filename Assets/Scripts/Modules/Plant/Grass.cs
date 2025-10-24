using System;
using TTGJ.Buff;
using TTGJ.GamePlay;

namespace TTGJ.Plant {
    public class Grass : PlantBase {
        public Action <int> OnGrassHarvested;
        public int index;
        public override void OnMature() { 

        }
        public override void OnGermination() { 

        }
        public override void OnWatering() { 

        }
        protected override void OnHarvest() { 
            OnGrassHarvested?.Invoke(index);
        }
        public override void OnEat() { 
            SheepTalkBuff buff = BuffManager.Instance.AddBuff<SheepTalkBuff>(PlayerController.Instance.transform);
            buff.StartBuff();
        }
    }
}
