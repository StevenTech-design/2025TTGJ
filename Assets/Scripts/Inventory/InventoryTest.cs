using System.Collections;
using System.Collections.Generic;
using TTGJ.Common;
using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    void Start()
    {
        // 自动添加测试物品
        InventorySystem.Instance.AddItem(1001, 5); // 药水 x5
        InventorySystem.Instance.AddItem(2001, 1); // 铁剑 x1
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            var items = InventorySystem.Instance.GetAllItems();
            Debug.Log($"📦 背包物品数: {items.Count}");
            foreach (var item in items)
            {
                var config = InventorySystem.Instance.GetItemConfig(item.itemId);
                Debug.Log($"UID: {item.uid} | {config?.name} x{item.count} (新: {item.isNew})");
            }
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            var items = InventorySystem.Instance.GetAllItems();
            if (items.Count > 0)
            {
                InventorySystem.Instance.UseItem(items[0].uid);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var items = InventorySystem.Instance.GetAllItems();
            if (items.Count > 0)
            {
                InventorySystem.Instance.RemoveItem(items[0].uid);
            }
        }
    }
}