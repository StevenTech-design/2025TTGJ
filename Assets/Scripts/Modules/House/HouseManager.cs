using System.Collections.Generic;
using cfg;
using TTGJ.Framework;
using TTGJ.Luban;
using TTGJ.Plant;
using UnityEngine;
namespace TTGJ.House {
    public class HouseManager : Singleton<HouseManager> {
        private List<HouseBase> _currentHouse = new List<HouseBase>();
        public GameObject CreateHouse(int id)
        {
            var item = LubanManager.Instance.GetItemNew(id);
            GameObject house = GameObject.Instantiate<GameObject>(StResources.Instance.LoadByResources<GameObject>(item.Model));
            _currentHouse.Add(house.GetComponent<HouseBase>());
            return house;
        }

        public void CollectPlant(PlantBase plant) {
            if (_currentHouse.Count < 1) {
                return;
            }
            foreach (var item in _currentHouse) {
                if (item.CheckFull()) {
                    continue;
                }
                var house = LubanManager.Instance.GetHouse((int)item.itemType);
                if (house.CropList.Contains((int)plant.itemType)) {
                    item.CollectPlant(plant);
                    return;
                }
            }
        }
    }
}