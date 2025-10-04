using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TTGJ.Framework
{
    public class StResources : Singleton<StResources>
    {
        private string _resourcesPathRoot = Application.dataPath + "/Res/";
        private IResourcesLoader _resourcesLoader;

        public async UniTask<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object
        {

            if (_resourcesLoader != null)
            {
                return await _resourcesLoader.LoadAsync<T>(assetPath);
            }
            return LoadInEditor<T>(assetPath);

        }

        private T LoadInEditor<T>(string assetPath) where T : UnityEngine.Object
        {
            string[] files = Directory.GetFiles(_resourcesPathRoot + Path.GetDirectoryName(assetPath),
                Path.GetFileName(assetPath) + ".*");

            if (files.Length > 0)
            {
                string fullPath = files[0];
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(fullPath);
                if (asset == null)
                {
                    Debug.LogWarning("Asset not found at " + fullPath);
                }

                return asset;
            }
            else
            {
                Debug.LogWarning("No matching files found for " + assetPath);
                return null;
            }
        }

        public void Release(UnityEngine.Object obj)
        {
            if (_resourcesLoader != null)
            {
                _resourcesLoader.Release(obj);
            }
        }
    }
}