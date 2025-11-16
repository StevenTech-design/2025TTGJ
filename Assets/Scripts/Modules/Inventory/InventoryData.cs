using System.Collections.Generic;
using TTGJ.Serialize;
using UnityEngine;

namespace TTGJ.Inventory { 
    public class InventoryData : ISerializable<InventoryData> { 

        private const int MAX_CAPACITY = 100;
        private Dictionary<int, InventoryItem> itemsDict;
        private Dictionary<int, int> itemsIndexDict;

        public InventoryData() { 
            itemsDict = new Dictionary<int, InventoryItem>();
            itemsIndexDict = new Dictionary<int, int>();
        }
        public bool AddItem(int itemId, int count) { 
            if (itemsDict.ContainsKey(itemId)) { 
                itemsDict[itemId].Count += count;
                return true;
            }
            int newIndex = GetNewIndex();
            if (newIndex == -1) { 
                return false;
            }
            itemsDict.Add(itemId, new InventoryItem(itemId, count, newIndex));
            itemsIndexDict.Add(newIndex, itemId);
            return true;
        }
        public void RemoveItem(int itemId, int count) { 
            if (itemsDict.ContainsKey(itemId)) { 
                itemsDict[itemId].Count -= count;
            }
        }
        public void MoveItem(int itemId, int fromIndex, int toIndex) {
            if (!itemsDict.ContainsKey(itemId)) { 
                return;
            }
            itemsDict[itemId].Index = toIndex;
            itemsIndexDict.Remove(fromIndex);
            itemsIndexDict.Add(toIndex, itemId);
        }

        private int GetNewIndex() { 
            for (int i = 0; i < MAX_CAPACITY; i++) { 
                if (!itemsIndexDict.ContainsKey(i)) { 
                    return i;
                }
            }
            return -1;
        }

       public string Serialize(InventoryData data)
        {
            var itemsList = new List<InventoryItem>();
            foreach (var item in data.itemsDict.Values)
            {
                itemsList.Add(item);
            }
            
            return JsonUtility.ToJson(itemsList.ToArray(), true);
        }

        public void Deserialize(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                return;
            }

            var itemsArray = JsonUtility.FromJson<InventoryItem[]>(jsonData);

            var inventory = new InventoryData();
            foreach (var item in itemsArray)
            {
                inventory.itemsDict[item.ItemId] = item;
                inventory.itemsIndexDict[item.Index] = item.ItemId;
            }
        }
    }
}