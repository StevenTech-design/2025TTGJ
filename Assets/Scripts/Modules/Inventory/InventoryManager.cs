using Steven.Framework;
using TTGJ.Manager;
using TTGJ.Storage;
using UnityEngine;

namespace TTGJ.Inventory
{
    public class InventoryManager : IManager
    {
        private const string BAG_INVENTORY_KEY = "player_bag_inventory";
        private const string STORAGE_HOUSE_INVENTORY_KEY = "player_storage_house_inventory";
        
        private InventoryData _bagInventoryData;
        private InventoryData _storageHouseInventoryData;
        
        private bool _isInitialized = false;
        public void Init()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("InventoryManager: 已初始化");
                return;
            }
            // 加载背包数据
            _bagInventoryData = LoadInventoryData(BAG_INVENTORY_KEY);
            
            // 加载仓库数据
            _storageHouseInventoryData = LoadInventoryData(STORAGE_HOUSE_INVENTORY_KEY);

            _isInitialized = true;
            Debug.Log("InventoryManager: 初始化完成");
        }

        private InventoryData LoadInventoryData(string key)
        {
            InventoryData data = null;
            StorageManager.Instance.LoadJson(key, out data);
            
            if (data == null)
            {
                Debug.Log($"InventoryManager: 创建新的库存数据 - Key: {key}");
                data = new InventoryData();
            }
            else
            {
                Debug.Log($"InventoryManager: 成功加载库存数据 - Key: {key}");
            }
            
            return data;
        }

        public void SaveAll()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("InventoryManager: 未初始化，无法保存");
                return;
            }

            bool bagSaved = StorageManager.Instance.SaveJson(BAG_INVENTORY_KEY, _bagInventoryData);
            bool storageSaved = StorageManager.Instance.SaveJson(STORAGE_HOUSE_INVENTORY_KEY, _storageHouseInventoryData);
            
            if (bagSaved && storageSaved)
            {
                Debug.Log("InventoryManager: 所有库存数据保存成功");
            }
            else
            {
                Debug.LogError("InventoryManager: 部分库存数据保存失败");
            }
        }

        public InventoryData GetBagInventory()
        {
            return _bagInventoryData;
        }

        public InventoryData GetStorageHouseInventory()
        {
            return _storageHouseInventoryData;
        }
        public bool IsInitialized()
        {
            return _isInitialized;
        }
    }
}