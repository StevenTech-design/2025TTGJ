using System.Collections.Generic;
using TTGJ.Common;
using UnityEngine;
using TTGJ.Buff;
using Unity.VisualScripting;
using TTGJ.Config;
using TTGJ.Audio;

namespace TTGJ.Plant
{
    public class WaterBeet : PlantBase
    {
        [SerializeField]
        private float wateringInterval = 0.5f;
        private float lastWateringTime = 0f;
        private void OnTriggerEnter(Collider other)
        {
            if(currentState < PlantState.Mature || Time.time - lastWateringTime < wateringInterval) { 
                return;
            }
            WateringOther();
            lastWateringTime = Time.time;
        }
        public override void OnTeleport(Transform baseTeleportPos) { 
            base.OnTeleport(baseTeleportPos);
            BuffBase buffBase = BuffManager.Instance.AddBuff<FieldWateringBuff>(transform);
            buffBase.StartBuff();
        }
        private void WateringOther()
        { 
            int currentOccupiedFieldSize = ConfigManager.Instance.GetOccupiedFieldSize(itemType, currentGrouthCount);
            List<PlantBase> plants = FieldSystem.Instance.GetSurroundPlants(GetComponent<Cell>().cellPos, currentOccupiedFieldSize);
            Debug.Log("WateringOther: " + plants.Count + "  " + GetComponent<Cell>().cellPos +"  "+currentOccupiedFieldSize);
            foreach (var plant in plants)
            {
                plant.OnWatering();
                Debug.Log("WateringOther: " + plant.gameObject.name + "  "+plant.GetComponent<Cell>().cellPos);
            }
            
        }
        private void OnCollisionStay(Collision collision)
        {
            if(currentState <= PlantState.Mature) { 
                return;
            }
            if (collision.transform.TryGetComponent<Cell>(out var cell))
            {
                AudioManager.Instance.PlaySFX(Audio.AudioType.Water_Beet_Falling_to_Ground);
                WateringOther();
                Destroy(gameObject);
            }
        }
        
    }
}