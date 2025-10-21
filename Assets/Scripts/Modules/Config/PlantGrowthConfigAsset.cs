using System.Collections.Generic;
using UnityEngine;
using TTGJ.Buff;
using TTGJ.Plant;
namespace TTGJ.Config
{
    [CreateAssetMenu(menuName = "TTGJ/Config/PlantGrowthConfigAsset", fileName = "PlantGrowthConfigAsset")]
    public class PlantGrowthConfigAsset : ScriptableObject
    {
        public List<PlantGrowthConfig> configs = new List<PlantGrowthConfig>();

        private Dictionary<ItemType, PlantGrowthConfig> _cache;


        public void RebuildCache()
        {
            _cache = new Dictionary<ItemType, PlantGrowthConfig>();
            foreach (var c in configs)
            {
                _cache[c.itemType] = c;
            }
        }

        public bool TryGet(ItemType type, out PlantGrowthConfig config)
        {
            if (_cache == null) RebuildCache();
            return _cache.TryGetValue(type, out config);
        }
    }
}