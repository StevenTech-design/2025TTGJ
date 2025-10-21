using System.Collections.Generic;
using UnityEngine;
using TTGJ.Buff;
namespace TTGJ.Config
{
    [CreateAssetMenu(menuName = "TTGJ/Config/BuffConfigAsset", fileName = "BuffConfigAsset")]
    public class BuffConfigAsset : ScriptableObject
    {
        public List<BuffConfig> configs = new List<BuffConfig>();

        private Dictionary<BuffType, float> _cache;


        public void RebuildCache()
        {
            _cache = new Dictionary<BuffType, float>();
            foreach (var c in configs)
            {
                _cache[c.buffType] = c.duration;
            }
        }

        public bool TryGet(BuffType type, out float duration)
        {
            if (_cache == null) RebuildCache();
            return _cache.TryGetValue(type, out duration);
        }
    }
}