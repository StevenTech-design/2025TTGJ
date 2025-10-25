using UnityEngine;
using System.Collections.Generic;
using TTGJ.Plant;
using System;
namespace TTGJ.Config
{
    [Serializable]
    public class PlantGrowthConfig
    {
        [Tooltip("作物")]
        public ItemType itemType;
        [Tooltip("每一阶段种植大小")]
        public List<Vector3> modelSize;
        [Tooltip("占据土地大小")]
        public List<int> occupiedFieldSize;
        [Tooltip("收获后大小")]
        public List<int> haverstSize;
    }
}