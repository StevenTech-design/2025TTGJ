using UnityEngine;
using System.Collections.Generic;
using TTGJ.Plant;
using System;
namespace TTGJ.Config
{
    [Serializable]
    public class PlantGrowthConfig
    {
        public ItemType itemType;
        public List<Vector3> modelSize;
        public List<int> occupiedFieldSize;
        public List<int> haverstSize;
    }
}