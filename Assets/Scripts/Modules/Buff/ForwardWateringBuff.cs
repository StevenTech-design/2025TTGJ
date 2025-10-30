using System;
using System.Collections.Generic;
using TTGJ.GamePlay;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.Buff
{
    public class ForwardWateringBuff : BuffBase
    {
        private LayerMask interactionLayers = ~0;
        private float interval = 0.5f;
        private float lastWateringTime = 0;
        protected override void OnBuffUpdate(float remainingTime)
        {
            if (Time.time - lastWateringTime >= interval)
            {
                ToWatering();
                lastWateringTime = Time.time;
            }
        }
        public void ToWatering()
        {
            var colliders = GetAdaptorObjs(CheckCanWatering);
            if (colliders == null || colliders.Count == 0)
            {
                return;
            }
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<Field>(out var field))
                {
                    field.ToWet();
                }
                else if (collider.gameObject.TryGetComponent<PlantBase>(out var plant))
                {
                    plant.OnWatering();
                }
            }
        }
        private List<Collider> GetAdaptorObjs(Func<Collider,bool> checkInteractable)
        {
            Vector3 center = PlayerController.GetActivePlayer().transform.position + PlayerController.GetActivePlayer().transform.forward * 0.2f;
            Vector3 halfExtents = new Vector3(0.3f, 1f, 0.25f);
            Collider[] colliders = Physics.OverlapBox(
                center,
                halfExtents,
                Quaternion.identity,
                interactionLayers,
                QueryTriggerInteraction.Collide
            );
            List<Collider> result = new();
            foreach (var collider in colliders)
            {
               var checkResult = checkInteractable.Invoke(collider);
               if(checkResult) { 
                result.Add(collider);
               }
            }
            return result;
        }
        private bool CheckCanWatering(Collider collider) {
            return collider.gameObject.TryGetComponent<Field>(out _) || collider.gameObject.TryGetComponent<PlantBase>(out var plant) 
                    && (plant.GetCurrentState() == PlantState.Mature || plant.GetCurrentState() == PlantState.Germination);
        }

    }
}