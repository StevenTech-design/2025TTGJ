using System.Collections.Generic;
using TTGJ.Framework;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Crafting
{
    public class CraftingTable : MonoBehaviour
    {
        private List<Liftable> liftables = new List<Liftable>();
        private void OnCollisionEnter(Collision other) {
            if (liftables.Count >= 2) {
                return;
            }
            if (other.gameObject.TryGetComponent<Liftable>(out var liftable))
            {
                liftables.Add(liftable);
            }
            if (liftables.Count == 2)
            { 
                ToCraft();
            }
        }
        private void ToCraft()
        {
            
        }
    }
}