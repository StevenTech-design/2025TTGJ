using System;
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

                // 初始化 Addressables（可选）
                Addressables.InitializeAsync().WaitForCompletion();

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

        public cfg.pokemon GetPokemon(int id)
        {
            CheckInitialized();
            return _tables.TbPokemon.GetOrDefault(id);
        }

        public cfg.move GetMove(int id)
        {
            CheckInitialized();
            return _tables.TbMove.GetOrDefault(id);
        }

        public cfg.item GetItem(int id)
        {
            CheckInitialized();
            return _tables.TbItem.GetOrDefault(id);
        }

        public cfg.map GetMap(int id)
        {
            CheckInitialized();
            return _tables.TbMap.GetOrDefault(id);
        }

        public cfg.instance GetInstance(int id)
        {
            CheckInitialized();
            return _tables.TbInstance.GetOrDefault(id);
        }

        public cfg.global GetGlobal(int id)
        {
            CheckInitialized();
            return _tables.TbGlobal.GetOrDefault(id);
        }

        public cfg.pokemonMove GetPokemonMove(int id)
        {
            CheckInitialized();
            return _tables.TbPokemonMove.GetOrDefault(id);
        }

        public cfg.typeChart GetTypeChart(int id)
        {
            CheckInitialized();
            return _tables.TbTypeChart.GetOrDefault(id);
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
                string fileName = $"Generate/Luban/{tableName}.json";

                // 1️⃣ 先尝试 Addressables
                try
                {
                    var handle = Addressables.LoadAssetAsync<TextAsset>(fileName);
                    var asset = handle.WaitForCompletion();

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

        #endregion
    }
}