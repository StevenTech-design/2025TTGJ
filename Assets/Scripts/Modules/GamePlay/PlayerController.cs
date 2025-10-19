using System.Collections.Generic;
using TTGJ.Plant;
using TTGJ.Interactable;
using UnityEngine;
using TTGJ.Framework;

namespace TTGJ.GamePlay
{
    public class PlayerController : MonoSingleton<PlayerController>
    {
        public float speed = 3.5f;
        [SerializeField]
        private float dropForce = 10;
        [SerializeField]
        private float dropAngle = 60f;
        [SerializeField]
        private float interactDistance = 2f; // 交互距离

        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private Queue<GameObject> _liftList = new();
        [SerializeField]
        private float listOffset = 0.5f;
        private float _currentHeight = 0;
        [SerializeField]
        private float _longTimeDropforce = 0;
        


        private void Awake()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
        }

        public void ToMove(Vector3 direction)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            _rigidbody.MovePosition(_rigidbody.position + speed * Time.fixedDeltaTime * transform.forward);
        }

        public void ToLift(GameObject target)
        {
            float targetHeight = 0;
            if (target.TryGetComponent<Collider>(out var collider))
            {
                collider.enabled = false;
            }
            if (target.TryGetComponent<MeshFilter>(out var filter))
            {
                targetHeight = filter.sharedMesh.bounds.size.y * target.transform.localScale.y;
            }

            if (target.TryGetComponent<Liftable>(out var liftable))
            {
                liftable.OnLift();
            }

            _liftList.Enqueue(target.gameObject);

            target.transform.SetParent(liftArea);
            float height = targetHeight / 2 + _currentHeight;
            target.transform.localRotation = Quaternion.identity;
            target.transform.localPosition = new Vector3(0, height, 0);
            _currentHeight += targetHeight + listOffset;
        }

        public void ToDrop()
        {
            Vector3 dropDirection = (transform.forward + Vector3.up).normalized;
            _liftList.Dequeue().GetComponent<Liftable>().OnDrop(dropDirection, dropForce);
            _currentHeight = 0;
        }
        public void LongTimeDrop() { 

        }
        public bool CanDrop()
        {
            return _liftList.Count > 0;
        }
        

        public void ToPlant(Field field)
        {
            if (field.IsPlanted()) return;
            PlantBase targetPlant = null;
            foreach (var plant in _liftList)
            {
                if (!plant.TryGetComponent<PlantBase>(out var plantBase))
                {
                    continue;
                }
                targetPlant = plantBase;
                break;
            }
            if (targetPlant == null || targetPlant.GetCurrentState() != PlantState.Seed) return;
            GameObject seed = GameObject.Instantiate(targetPlant.gameObject);
            seed.GetComponent<Collider>().enabled = true;
            field.ToPlanting(seed.GetComponent<PlantBase>());
        }
        public void ToEat()
        {
            GameObject target = _liftList.Peek();
            if (target.TryGetComponent<IEatable>(out var eatable) && eatable.CanEat())
            {
                eatable.OnEat();
                ObjectPoolManager.Instance.ReturnGameObjectToPool(target);
                _liftList.Dequeue();
            }
            RefreshLiftQueue();
        }
        public GameObject GetLiftObject() { 
            return _liftList.Peek();
        }
        private void RefreshLiftQueue() { 
            _currentHeight = 0;
            foreach (var target in _liftList)
            {
                target.transform.localPosition = new Vector3(0, _currentHeight, 0);
                _currentHeight += 0.5f;
            }
        }
    }
}


