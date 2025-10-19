using UnityEngine;
using TTGJ.Plant;
using TTGJ.Interactable;
using System;

namespace TTGJ.GamePlay
{
    public class Kettle : Liftable
    {
        [SerializeField]
        private LayerMask interactionLayers = ~0;
        public void ToWatering() { 
            Collider collider = GetBestAdaptorObj(CheckCanWatering);
            if (collider == null) { 
                return;
            }
            _ = collider.gameObject.GetComponent<PlantBase>().OnWatering();
        }
        private Collider GetBestAdaptorObj(Func<Collider,bool> checkInteractable)
        {
            Vector3 center = PlayerController.Instance.transform.position + PlayerController.Instance.transform.forward * 0.2f;
            Vector3 halfExtents = new Vector3(0.3f, 1f, 0.25f);
            Collider[] colliders = Physics.OverlapBox(
                center,
                halfExtents,
                Quaternion.identity,
                interactionLayers,
                QueryTriggerInteraction.Collide
            );

            Array.Sort(colliders, (a, b) => (a.transform.position - center).sqrMagnitude.CompareTo((b.transform.position - center).sqrMagnitude));
            foreach (var collider in colliders)
            {
               var checkResult = checkInteractable.Invoke(collider);
               if(checkResult) { 
                return (collider);
               }
            }
            return null;
        }
        private bool CheckCanWatering(Collider collider) { 
            return collider.gameObject.TryGetComponent<PlantBase>(out var plant) 
                    && (plant.GetCurrentState() == PlantState.Mature || plant.GetCurrentState() == PlantState.Germination);
        }
    }
}