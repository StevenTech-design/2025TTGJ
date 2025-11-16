using System;
using Cysharp.Threading.Tasks;

namespace Steven.Framework
{
    public interface IResourcesLoader
    {
        UniTask<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object;
        void Release(UnityEngine.Object obj);
    }
}