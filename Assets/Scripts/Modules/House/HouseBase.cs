using System.Collections;
using System.Collections.Generic;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using TTGJ.Luban;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.House
{
    public class HouseBase : Liftable, IEatable
    {
        [SerializeField]
        private int maxCount = 50;

        private Queue<PlantBase> _currentStorePlants = new Queue<PlantBase>();

        public bool CanEat()
        {
            return true;
        }

        public bool CheckFull()
        {
            return _currentStorePlants.Count >= maxCount;
        }
        public void CollectPlant(PlantBase plantBase)
        {
            _currentStorePlants.Enqueue(plantBase);
            plantBase.GetComponent<Rigidbody>().velocity = Vector3.zero;
            plantBase.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            plantBase.gameObject.SetActive(false);
        }

        public void OnEat()
        {
            if (_currentStorePlants == null || _currentStorePlants.Count < 1)
            {
                //TODO play audio
                return;
            }
            PlantBase go = _currentStorePlants.Dequeue();
            go.gameObject.SetActive(false);
            go.transform.SetPositionAndRotation(transform.position, transform.rotation);
            go.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * PlayerController.GetActivePlayer().dropForce, ForceMode.Impulse);
        }
        // private float ChangeState() { 

        // }


    }

}