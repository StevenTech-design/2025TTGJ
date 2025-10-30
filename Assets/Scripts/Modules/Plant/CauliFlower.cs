using System.Collections.Generic;
using TTGJ.Audio;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Plant
{
    public class CauliFlower : PlantBase
    {
        [SerializeField]
        private GameObject sheepPrefab;
        private void OnTriggerEnter(Collider other)
        {
            if(currentState != PlantState.Mature) { 
                return;
            }
            Debug.Log("CauliFlower OnTriggerEnter  mei mei");
        }

        protected override void OnHarvest()
        {
            base.OnHarvest();
            Destroy(modelRoot.GetChild(0).gameObject);
            GameObject sheep = GameObject.Instantiate(sheepPrefab, transform.position, transform.rotation);
            sheep.transform.SetParent(modelRoot);
            sheep.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            sheep.transform.localScale = Vector3.one;
            AudioManager.Instance.PlaySFX(Audio.AudioType.Harvesting_Strange_Crop_2);
        }
        public override void OnPlant()
        {
            base.OnPlant();
            GameObject peekObject = PlayerController.Instance.GetPeekGameObject();
            PlayerController.Instance.ToDrop();
            Destroy(peekObject);
        }

         
    }
}