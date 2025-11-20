using System.Collections.Generic;
using TTGJ.UI;
using UnityEngine;

namespace TTGJ.UI
{
    public class InventoryPanel : UIPanel
    {
        [SerializeField] private Transform content;
        [SerializeField] private GameObject inventoryItemTemplate;
        [SerializeField] private GameObject boxItemTemplate;
        private List<GameObject> _boxItems = new List<GameObject>();

        private void Init(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                GameObject item = Instantiate(inventoryItemTemplate, content);
                _boxItems.Add(item);
            }
        }

        private void ShowData(Dictionary<int, Inventory.InventoryItem> inventoryDic)
        {
            if (inventoryDic == null || inventoryDic.Count == 0)
            {
                return;
            }

            foreach (var item in inventoryDic)
            {
                GameObject go = Instantiate(inventoryItemTemplate, _boxItems[item.Key].transform);
                go.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                go.GetComponent<InventoryItem>().RefreshInfo(item.Value);

            }
        }
    }
}
