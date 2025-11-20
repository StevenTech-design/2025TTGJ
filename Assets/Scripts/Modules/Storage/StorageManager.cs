using System;
using System.IO;
using UnityEngine;
using Steven.Framework;
using TTGJ.Serialize;
using TTGJ.Manager;

namespace TTGJ.Storage
{
    public class StorageManager : Singleton<StorageManager>, IManager
    {
        private bool _isInitialized = false;
        private string _saveDirectory; 
        private string _saveFolderName = "SaveData";

        public void Init()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("StorageManager: 已初始化");
                return;
            }

            _saveDirectory = Path.Combine(Application.persistentDataPath, _saveFolderName);
            
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }

            _isInitialized = true;
            Debug.Log($"StorageManager: 初始化完成");
            Debug.Log($"  存档目录: {_saveDirectory}");
        }

        private void CheckInitialized()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("StorageManager: 未初始化，自动初始化...");
                Init();
            }
        }

        private string GetFilePath(string key, string extension)
        {
            // ✅ 使用完整路径
            return Path.Combine(_saveDirectory, $"{key}{extension}");
        }

        /// <summary>
        /// 保存实现了 ISerializable 接口的对象（JSON 格式）
        /// </summary>
        public bool SaveJson<T>(string key, T data) where T : class, ISerializable<T>
        {
            CheckInitialized();

            if (string.IsNullOrEmpty(key) || data == null)
            {
                Debug.LogError("StorageManager: 参数不能为空");
                return false;
            }

            try
            {
                // ✅ 使用 ISerializable 接口序列化为 JSON 字符串
                string jsonString = data.Serialize(data);

                // ✅ 保存为 .json 文件
                string filePath = GetFilePath(key, ".json");
                File.WriteAllText(filePath, jsonString);

                Debug.Log($"StorageManager: ✅ 保存成功");
                Debug.Log($"  Key: {key}");
                Debug.Log($"  Path: {filePath}");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"StorageManager: ❌ 保存失败 - Key: {key}");
                Debug.LogError($"  Error: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        public void LoadJson<T>(string key, out T result) where T : ISerializable<T>,new()
        {
            CheckInitialized();
            result = new T();
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("StorageManager: 参数不能为空");
                return;
            }

            string filePath = GetFilePath(key, ".json");
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"StorageManager: 存档不存在 - Key: {key}");
                return;
            }

            try
            {
                // ✅ 读取 JSON 字符串
                string jsonString = File.ReadAllText(filePath);
                
                // ✅ 使用 ISerializable 接口反序列化
                result.Deserialize(jsonString);

                Debug.Log($"StorageManager: ✅ 加载成功 - Key: {key}");
            }
            catch (Exception e)
            {
                Debug.LogError($"StorageManager: ❌ 加载失败 - Key: {key}");
                Debug.LogError($"  Error: {e.Message}");
                return;
            }
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool Exists(string key)
        {
            CheckInitialized();
            string filePath = GetFilePath(key, ".json");
            return File.Exists(filePath);
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public bool Delete(string key)
        {
            CheckInitialized();

            string filePath = GetFilePath(key, ".json");
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"StorageManager: 存档不存在 - Key: {key}");
                return false;
            }

            try
            {
                File.Delete(filePath);
                Debug.Log($"StorageManager: ✅ 删除成功 - Key: {key}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"StorageManager: ❌ 删除失败 - {e.Message}");
                return false;
            }
        }
    }
}