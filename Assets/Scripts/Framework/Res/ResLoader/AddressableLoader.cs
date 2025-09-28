using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TTGJ.Framework
{
    public class AddressableLoader : IResourcesLoader
    {
        private Dictionary<Object, AsyncOperationHandle> _addressablesHandles = new Dictionary<Object, AsyncOperationHandle>();
        public async UniTask<T> Load<T>(string assetPath) where T : Object
        {
            T res = null;
            if (typeof(T) != typeof(GameObject))
            {
                var handle = Addressables.LoadAssetAsync<T>(assetPath);
                res = await handle.Task;
                _addressablesHandles[handle.Result] = handle;
            }
            else
            {
                var handle = Addressables.InstantiateAsync(assetPath);
                res = await handle.Task as T;
                _addressablesHandles[handle.Result] = handle;
            }
            return res;
        }

        public void Release(Object obj)
        {
            if (!_addressablesHandles.TryGetValue(obj, out var handle))
            {
                if (obj is GameObject) GameObject.Destroy(obj);
                return;
            }

            _addressablesHandles.Remove(obj);

            if (obj is GameObject)
            {
                GameObject.Destroy(obj);
                Addressables.ReleaseInstance(obj as GameObject);
                return;
            }

            Addressables.Release(handle);
        }
    }
}