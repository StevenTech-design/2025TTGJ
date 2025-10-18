using System.Collections.Generic;
using TTGJ.Framework;
using UnityEngine;

namespace TTGJ.Crafting
{
    public class CraftingTable : MonoBehaviour
    {
        List<ICraftable> craftables = new List<ICraftable>();
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<ICraftable>(out var craftable))
            {
                craftables.Add(craftable);
                ObjectPoolManager.Instance.ReturnGameObjectToPool(other.gameObject);
            }
        }
        private void ToCraft() { 
            Debug.Log("ToCraft");
            craftables.Clear();
            Debug.Log("ToCraft: Finished");
        }
    }
}