using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TTGJ.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Framework.Res.ResLoader
{
    public class AddressableLoader : IResourcesLoader
    {
        private readonly Dictionary<Object, AsyncOperationHandle> _addressablesHandles = new();

        public async UniTask<T> LoadAsync<T>(string assetPath) where T : Object
        {
            T res;
            if (typeof(T) != typeof(GameObject))
            {
                AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetPath);
                res = await handle.Task;
                _addressablesHandles[handle.Result] = handle;
            }
            else
            {
                AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(assetPath);
                res = await handle.Task as T;
                _addressablesHandles[handle.Result] = handle;
            }
            return res;
        }

        public void Release(Object obj)
        {
            if (!_addressablesHandles.TryGetValue(obj, out AsyncOperationHandle handle))
            {
                if (obj is GameObject)
                {
                    Object.Destroy(obj);
                }

                return;
            }

            _ = _addressablesHandles.Remove(obj);

            if (obj is GameObject)
            {
                Object.Destroy(obj);
                _ = Addressables.ReleaseInstance(obj as GameObject);
                return;
            }

            Addressables.Release(handle);
        }
    }
}