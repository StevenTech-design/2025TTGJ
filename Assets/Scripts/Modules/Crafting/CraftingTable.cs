using System;
using System.Collections.Generic;
using TTGJ.Framework;
using TTGJ.Generate;
using TTGJ.Interactable;
using TTGJ.Luban;
using UnityEngine;
using TTGJ.Config;
using TTGJ.House;
using Cysharp.Threading.Tasks;
using TTGJ.Audio;
using AudioType = TTGJ.Audio.AudioType;

namespace TTGJ.Crafting
{
    public class CraftingTable : MonoBehaviour
    {
        private List<Liftable> liftables = new List<Liftable>();
        [SerializeField]
        private Animator animator;
        private Transform craftPosition;
        [SerializeField]
        private CraftingTableType craftingTableType;
        private void OnCollisionEnter(Collision other)
        {
            if (liftables.Count >= 2)
            {
                return;
            }
            if (other.gameObject.TryGetComponent<Liftable>(out var liftable))
            {
                liftables.Add(liftable);
                liftable.gameObject.SetActive(false);
                animator.SetTrigger("eat");
                AudioManager.Instance.PlaySFX(AudioType.Popcorn_Machine_Loading_Crops);
            }
            if (liftables.Count == 2)
            {
                ToCraft();
            }
        }
        private async UniTask ToCraft()
        {
            await UniTask.Delay(500);
            animator.SetTrigger("making");
            AudioManager.Instance.PlaySFX(AudioType.Popcorn_Machine_Sound_1);
            await UniTask.Delay(4000);
            var obj = GetTargetCraftObject();
            if(obj == null) {
                animator.SetTrigger("finish");
                AudioManager.Instance.PlaySFX(AudioType.Popcorn_Machine_Dispensing);
                liftables[0].gameObject.SetActive(true);
                await UniTask.Delay(500);
                animator.SetTrigger("finish");
                AudioManager.Instance.PlaySFX(AudioType.Popcorn_Machine_Dispensing);
                liftables[1].gameObject.SetActive(true);
                liftables[0].transform.position = transform.position + transform.forward * 2f;
                liftables[1].transform.position = transform.position + transform.forward * 3f;
                liftables.Clear();
                return;
            }
            animator.SetTrigger("finish");
            obj.transform.position = transform.position + transform.forward * 2f;
            liftables.Clear();
        }
        private GameObject GetTargetCraftObject()
        {
            var combineItem = LubanManager.Instance.GetCombineItem((int)craftingTableType,(int)liftables[0].itemType, (int)liftables[1].itemType);
            if (combineItem == null) {
                Debug.LogError("No combine item id: " + liftables[0].itemType + " " + liftables[1].itemType);
                return null;
            }
            var itemNew = LubanManager.Instance.GetItemNew(combineItem.RewardBoxId);

             if(itemNew == null) {
                Debug.LogError("No item new id: " + combineItem.RewardBoxId);
                return null;
             }
            GameObject res = null;
            if (itemNew.ItemType == (int)ItemCatagory.Room)
            {
                res = HouseManager.Instance.CreateHouse(itemNew.Id);
            }
            else
            {
                res = Instantiate(StResources.Instance.LoadByResources<GameObject>(itemNew.Model));
            }

             return res;
        }
    }
}