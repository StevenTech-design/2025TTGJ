using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TTGJ.Common
{
    public class InventorySystem : MonoBehaviour
    {
        public static InventorySystem Instance { get; private set; }

        [SerializeField] private List<InventoryItemInstance> items = new();

        // 模拟物品配置表（实际项目中应由 Luban 生成）
        private static Dictionary<int, InventoryItemData> itemDatabase = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadMockData(); // 加载模拟数据
                LoadSavedData(); // 从 PlayerPrefs 加载存档
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // ──────────────── 模拟数据（临时用）────────────────
        private void LoadMockData()
        {
            // 示例：ID=1001 是药水，ID=2001 是武器
            itemDatabase[1001] = new InventoryItemData
            {
                id = 1001,
                name = "治疗药水",
                description = "恢复50点生命值",
                maxStack = 99,
                canUse = true,
                canSell = true,
                sellPrice = 20,
                iconPath = "Icons/Potion"
            };
            itemDatabase[2001] = new InventoryItemData
            {
                id = 2001,
                name = "铁剑",
                description = "基础武器",
                maxStack = 1,
                canUse = false,
                canSell = true,
                sellPrice = 100,
                iconPath = "Icons/Sword"
            };
        }

        // ──────────────── 存档系统 ────────────────
        private const string SAVE_KEY = "InventoryData";

        public void Save()
        {
            string json = JsonUtility.ToJson(new InventorySaveData { items = items });
            PlayerPrefs.SetString(SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void LoadSavedData()
        {
            if (PlayerPrefs.HasKey(SAVE_KEY))
            {
                string json = PlayerPrefs.GetString(SAVE_KEY);
                var saveData = JsonUtility.FromJson<InventorySaveData>(json);
                items = saveData.items ?? new List<InventoryItemInstance>();
            }
        }

        // ──────────────── 对外接口 ────────────────

        public void AddItem(int itemId, int count = 1)
        {
            if (!itemDatabase.ContainsKey(itemId))
            {
                Debug.LogError($"[Inventory] 物品ID {itemId} 不存在！");
                return;
            }

            var config = itemDatabase[itemId];
            var existing = FindItem(itemId);

            if (existing != null && existing.count < config.maxStack)
            {
                int canAdd = Mathf.Min(count, config.maxStack - existing.count);
                existing.count += canAdd;
                count -= canAdd;
            }

            if (count > 0)
            {
                items.Add(new InventoryItemInstance
                {
                    uid = System.Guid.NewGuid().ToString(),
                    itemId = itemId,
                    count = count,
                    isNew = true
                });
            }

            Save();
        }

        public void RemoveItem(string uid)
        {
            items.RemoveAll(item => item.uid == uid);
            Save();
        }

        public void UseItem(string uid)
        {
            var item = items.Find(i => i.uid == uid);
            if (item == null) return;

            var config = GetItemConfig(item.itemId);
            if (config?.canUse != true) return;

            // TODO: 执行使用逻辑
            Debug.Log($"使用物品: {config.name}");

            // 使用后数量减1
            item.count--;
            if (item.count <= 0)
            {
                RemoveItem(uid);
            }
            else
            {
                Save();
            }
        }

        public List<InventoryItemInstance> GetAllItems() => new List<InventoryItemInstance>(items);

        public InventoryItemData GetItemConfig(int itemId)
        {
            itemDatabase.TryGetValue(itemId, out var config);
            return config;
        }

        private InventoryItemInstance FindItem(int itemId)
        {
            return items.Find(item => item.itemId == itemId);
        }

    }

    // 用于序列化存档
    [Serializable]
    internal class InventorySaveData
    {
        public List<InventoryItemInstance> items;
    }

}