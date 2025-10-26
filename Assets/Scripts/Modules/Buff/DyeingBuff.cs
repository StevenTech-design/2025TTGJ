using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;
namespace TTGJ.Buff
{
    public class DyeingBuff : BuffBase
    {
        private int colorID;
        [SerializeField]
        private LayerMask dyeingLayerMask;
        public override void StartBuff()
        {
            base.StartBuff();
            Dyeing();
        }
        private void Dyeing()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10, LayerMask.GetMask("Default"));
            foreach (Collider collider in colliders)
            {
                IDyeingable dyeingable = collider.GetComponent<IDyeingable>();
                if (dyeingable != null)
                {
                    dyeingable.Dyeing(colorID);
                }
            }
        }
    }
}