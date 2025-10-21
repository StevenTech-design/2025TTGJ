using System.Collections.Generic;
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
            var combineItem = GetTargetCraftObject();
            if (combineItem != null)
            {
                Debug.Log("ToCraft: " + combineItem.Name);
            }
            else
            { 
                Debug.Log("No combine item back to player");
            }
            liftables.Clear();
        }
        private cfg.ItemCombine GetTargetCraftObject()
        {
            var combineItem = LubanManager.Instance.GetCombineItem((int)liftables[0].itemType, (int)liftables[1].itemType);
            Debug.Log(combineItem);
            return combineItem;
            
        }
    }
}