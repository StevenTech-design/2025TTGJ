using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Carrot : PlantBase
    {
        public override void OnWatering()
        {
            base.OnWatering();
            transform.localScale = new Vector3(growthScale[currentGrouthCount - 1], 5, growthScale[currentGrouthCount - 1]);
        }
        public override void OnMature()
        {
            transform.localScale = new Vector3(1f, 5f, 1f);
        }
    }
}