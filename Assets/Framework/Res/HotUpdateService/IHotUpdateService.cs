using System;
using Cysharp.Threading.Tasks;

namespace Steven.Framework
{
    public interface IHotUpdateService
    { 
        UniTask<bool> CheckResourceUpdate();
        UniTask<bool> UpdateResource(Action<float,float> onProgress);
    }
}