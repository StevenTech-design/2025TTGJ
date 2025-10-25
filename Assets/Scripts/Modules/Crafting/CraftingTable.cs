using System;
using System.Collections.Generic;
using TTGJ.Framework;
using TTGJ.Generate;
using TTGJ.Interactable;
using TTGJ.Luban;
using UnityEngine;

namespace TTGJ.Crafting
{
    public class CraftingTable : MonoBehaviour
    {
        private List<Liftable> liftables = new List<Liftable>();
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
            }
            if (liftables.Count == 2)
            {
                ToCraft();
            }
        }
        private void ToCraft()
        {
            var stringPath = GetTargetCraftObject();
            if(string.IsNullOrEmpty(stringPath)) {
                liftables[0].gameObject.SetActive(true);
                liftables[1].gameObject.SetActive(true);
                liftables[0].transform.position = transform.position + transform.forward * 2f;
                liftables[1].transform.position = transform.position + transform.forward * 3f;
                liftables.Clear();
                return;
            }
            var obj = GameObject.Instantiate(StResources.Instance.LoadByResources<GameObject>(stringPath));
            obj.transform.position = transform.position + transform.forward * 2f;
            liftables.Clear();
        }
        private string GetTargetCraftObject()
        {
            var combineItem = LubanManager.Instance.GetCombineItem((int)liftables[0].itemType, (int)liftables[1].itemType);
            if (combineItem == null) {
                Debug.LogError("No combine item id: " + liftables[0].itemType + " " + liftables[1].itemType);
                return null;
            }
             var itemNew = LubanManager.Instance.GetItemNew(combineItem.RewardBoxId);

             if(itemNew == null) {
                Debug.LogError("No item new id: " + combineItem.RewardBoxId);
                return null;
             }

             return itemNew.Model;
             

        }
    }
}