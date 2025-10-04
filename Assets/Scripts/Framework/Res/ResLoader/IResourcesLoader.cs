using System;
using Cysharp.Threading.Tasks;

namespace TTGJ.Framework
{
    public interface IResourcesLoader
    {
        UniTask<T> LoadAsync<T>(string assetPath) where T : UnityEngine.Object;
        void Release(UnityEngine.Object obj);
    }
}