using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SimpleJSON;
using TTGJ.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TTGJ.Luban
{
    /// <summary>
    /// Luban数据表管理器
    /// 负责初始化和提供数据表访问接口
    /// </summary>
    public class LubanManager : Singleton<LubanManager>
    {
        #region Private Fields

        private cfg.Tables _tables;
        private bool _isInitialized = false;

        #endregion

        #region Public Properties

        public bool IsInitialized => _isInitialized;

        #endregion

        #region Public Methods

        public async UniTask InitializeAsync()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("LubanManager: 数据表已初始化");
                return;
            }

            try
            {
                Debug.Log("LubanManager: 开始初始化数据表...");

                var loader = CreateJsonLoader();

                _tables = new cfg.Tables(loader);
                _isInitialized = true;

                Debug.Log("LubanManager: 数据表初始化完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"LubanManager 初始化失败: {e}");
                throw;
            }
        }

        #endregion

        #region Private Methods

        private void CheckInitialized()
        {
            if (!_isInitialized)
                throw new Exception("LubanManager: 数据表未初始化，请先调用 InitializeAsync()");
        }

        /// <summary>
        /// 创建 JSON 加载器
        /// 优先尝试从 Addressables 加载，否则从 Resources 加载
        /// </summary>
        private Func<string, JSONNode> CreateJsonLoader()
        {
            return (string tableName) =>
            {
                string fileName = $"Generate/Luban/{tableName}";

                // 1️⃣ 先尝试 Addressables
                try
                {
                    var asset = StResources.Instance.LoadByResources<TextAsset>(fileName);
                    

                    if (asset != null)
                        return JSON.Parse(asset.text);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Addressables 未找到 {fileName}，将尝试从 Resources 加载。 ({e.Message})");
                }

                // 2️⃣ 再尝试 Resources
                var resourcePath = $"Generate/Luban/{tableName}";
                var textAsset = Resources.Load<TextAsset>(resourcePath);

                if (textAsset == null)
                    throw new Exception($"无法加载数据表: {tableName} (Addressables 与 Resources 都未找到)");

                return JSON.Parse(textAsset.text);
            };
        }

        public cfg.ItemNew GetItemNew(int id)
        {
            return _tables.TbItemNew.Get(id);
        }
        private cfg.TBItemCombine GetTBItemCombine()
        {
            return _tables.TBItemCombine;
        }
        public cfg.ItemCombine GetItemCombine(int id)
        {
            return _tables.TBItemCombine.Get(id);
        }
        public cfg.ItemCombine GetCombineItem(int itemId1, int itemId2)
        {
            var itemCombine = GetTBItemCombine();
            foreach (var item in itemCombine.DataList)
            {
                if ((item.CostMainBuckets1 == itemId1 && item.CostMainBuckets2 == itemId2)
                || item.CostMainBuckets1 == itemId2 && item.CostMainBuckets2 == itemId1)
                {
                    return item;
                }
            }
            return null;
        }
        public int EvolutionId(int id)
        {
            var item = GetItemNew(id);
            if (item == null)
            {
                return -1;
            }
            return item.EvoId;
        }
        public cfg.plot GetPlot(int id)
        {
            return _tables.TbPlot.Get(id);
        }
        public cfg.task GetTask(int id)
        {
            return _tables.TbTask.Get(id);
        }
        public List<int> GetItemsByType(int itemType) { 
            var items = _tables.TbItemNew.DataList;
            return items.Where(item => item.ItemType == itemType).Select(item => item.Id).ToList();
        }


        #endregion
    }
}