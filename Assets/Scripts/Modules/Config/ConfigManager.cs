using System.Collections.Generic;
using TTGJ.Buff;
using TTGJ.Framework;
using TTGJ.Generate;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.Config
{
    public class ConfigManager : Singleton<ConfigManager>
    {
        private PlantGrowthConfigAsset plantGrowthConfigs;
        private BuffConfigAsset buffConfigs;
        private List<int> defaultOccupiedFieldSize = new List<int> { 1, 1, 3, 3, 3 };
        private List<int> defaultHaverstSize = new List<int> { 1, 2, 2, 2, 5 };
        private List<float> defaultGrowthSize = new List<float> { 1f, 2f, 3f, 4f, 5f };
        public void Init()
        {
            InitBuffConfigs();
            InitPlantGrowthConfigs();
        }
        private void InitPlantGrowthConfigs()
        {
            plantGrowthConfigs = StResources.Instance.LoadByResources<PlantGrowthConfigAsset>(ResPathConfig.Config_PlantGrowthConfigAsset);
            plantGrowthConfigs.RebuildCache();
            Debug.Log("InitPlantGrowthConfigs: " + plantGrowthConfigs.configs.Count);
        }
        private void InitBuffConfigs()
        {
            buffConfigs = StResources.Instance.LoadByResources<BuffConfigAsset>(ResPathConfig.Config_BuffConfigAsset);
            buffConfigs.RebuildCache();
        }
        #region Buff Configs
        public float GetBuffDuration(BuffBase buff)
        {
            if (buffConfigs == null)
            {
                InitBuffConfigs();
            }
            if (!buffConfigs.TryGet(buff.GetBuffType(), out float duration))
            {
                return 30f;
            }
            return duration;
        }
        public float GetBuffCheckInterval(BuffBase buff)
        {
            if (buffConfigs == null)
            {
                InitBuffConfigs();
            }
            if (!buffConfigs.TryGet(buff.GetBuffType(), out float checkInterval))
            {
                return 0.1f;
            }
            return checkInterval;
        }
        #endregion

        #region Plant Growth Configs
        public Vector3 GetPlantGrowthModelSize(ItemType itemType, int growthCount)
        {
            if (plantGrowthConfigs == null)
            {
                InitPlantGrowthConfigs();
            }

            if (!plantGrowthConfigs.TryGet(itemType, out PlantGrowthConfig config))
            {
                return Vector3.one * (growthCount < defaultGrowthSize.Count ? defaultGrowthSize[growthCount - 1] : defaultGrowthSize[^1]);
            }
            return config.modelSize[growthCount - 1];
        }
        public int GetOccupiedFieldSize(ItemType itemType, int growthCount)
        {
            if(growthCount <= 0) { 
                return 1;
            }
            if (plantGrowthConfigs == null)
            {
                InitPlantGrowthConfigs();
            }
            if (!plantGrowthConfigs.TryGet(itemType, out PlantGrowthConfig config))
            {
                return growthCount < defaultOccupiedFieldSize.Count
                ? defaultOccupiedFieldSize[growthCount -1]
                : defaultOccupiedFieldSize[^1];
            }
            return config.occupiedFieldSize[growthCount - 1];
        }
        public int GetHaverstSize(ItemType itemType, int growthCount)
        {
            if (plantGrowthConfigs == null)
            {
                InitPlantGrowthConfigs();
            }
            if (!plantGrowthConfigs.TryGet(itemType, out PlantGrowthConfig config))
            {
                return growthCount - 1 < defaultHaverstSize.Count
                ? defaultHaverstSize[growthCount - 1]
                : defaultHaverstSize[defaultHaverstSize.Count - 1];
            }
            return config.haverstSize[growthCount - 1];
        }
        public int GetGrowthCount(ItemType itemType)
        {
            if (plantGrowthConfigs == null)
            {
                InitPlantGrowthConfigs();
            }
            if (!plantGrowthConfigs.TryGet(itemType, out PlantGrowthConfig config))
            {
                return defaultOccupiedFieldSize.Count;
            }
            return config.modelSize.Count;
        }
        #endregion
    }
}