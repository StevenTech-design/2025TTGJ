using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;
namespace TTGJ.Buff
{
    public class DyeingBuff : BuffBase
    {
        [SerializeField]
        private Color dyeingColor;
        [SerializeField]
        private LayerMask dyeingLayerMask;
        protected override void StartBuff()
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
                    dyeingable.Dyeing(dyeingColor);
                }
            }
        }
    }
}