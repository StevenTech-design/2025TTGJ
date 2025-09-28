using System;
using Cysharp.Threading.Tasks;

namespace TTGJ.Framework
{
    public interface IResourcesLoader
    {
        UniTask<T> Load<T>(string assetPath) where T : UnityEngine.Object;
        void Release(UnityEngine.Object obj);
    }
}