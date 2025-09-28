using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TTGJ.Framework
{
    public class AddressableHotUpdateService : IHotUpdateService
    {
        public async UniTask<bool> CheckResourceUpdate()
        {
            AsyncOperationHandle<List<string>> handle = Addressables.CheckForCatalogUpdates();
            List<string> catalogs = await handle.Task;
            Addressables.Release(handle);
            return catalogs != null && catalogs.Count > 0;
        }

        public async UniTask<bool> UpdateResource(System.Action<float, float> onProgress)
        {
            var resourceLocator = await Addressables.InitializeAsync();
            var allKeys = resourceLocator.Keys;
            var totalDownloadSize = await Addressables.GetDownloadSizeAsync(allKeys);
            var downloadedSize = 0f;
            foreach (var key in allKeys)
            {
                var downloadSize = await Addressables.GetDownloadSizeAsync(key);
                if (downloadSize <= 0) continue;
                var keyDownloadOpration = Addressables.DownloadDependenciesAsync(key);
                while (!keyDownloadOpration.IsDone)
                {
                    await UniTask.Yield();
                    var acquiredKb = downloadedSize + (keyDownloadOpration.PercentComplete * downloadSize);
                    onProgress?.Invoke(acquiredKb, totalDownloadSize);
                }
            }
            return true;
        }
    }
}