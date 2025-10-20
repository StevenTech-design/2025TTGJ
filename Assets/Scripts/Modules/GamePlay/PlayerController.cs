using System.Collections.Generic;
using TTGJ.Plant;
using TTGJ.Interactable;
using UnityEngine;
using TTGJ.Framework;

namespace TTGJ.GamePlay
{
    public partial class PlayerController : MonoSingleton<PlayerController>
    {
        public float speed = 3.5f;
        [SerializeField]
        private float dropForce = 10;
        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private Queue<GameObject> _liftList = new();
        [SerializeField]
        private float listOffset = 1f;
        private float _currentHeight = 0;
        [SerializeField]
        private float _longTimeDropforce = 0;



        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _animator = GetComponentInChildren<Animator>();
        }

        public void ToMove(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                IsMove = false;
                return;
            }
            IsMove = true;
            transform.rotation = Quaternion.LookRotation(direction);
            _rigidbody.MovePosition(_rigidbody.position + speed * Time.fixedDeltaTime * transform.forward);
        }

        public void ToLift(GameObject target)
        {
            if (target.TryGetComponent<Liftable>(out var liftable))
            {
                liftable.OnLift();
            }
            _liftList.Enqueue(target.gameObject);

            target.transform.SetParent(liftArea);
            target.transform.SetLocalPositionAndRotation(new Vector3(0, _currentHeight, 0), Quaternion.identity);
            _currentHeight += listOffset;
            PlayLiftAnimation();
            IsLift = _liftList.Count > 0;
        }

        public void ToDrop()
        {
            Vector3 dropDirection = (transform.forward + Vector3.up).normalized;
            var target =_liftList.Dequeue();
            target.GetComponent<Liftable>().OnDrop(dropDirection, dropForce);
            target.transform.SetParent(null);
            IsLift = _liftList.Count > 0;
            RefreshLiftQueue();
        }
        public void LongTimeDrop()
        {
            Vector3 dropDirection = (transform.forward + Vector3.up).normalized;
            while (_liftList.Count > 0)
            {
                GameObject target = _liftList.Dequeue();
                target.transform.SetParent(null);
                if (target.TryGetComponent<Liftable>(out var liftable))
                {
                    liftable.OnDrop(dropDirection, dropForce);
                }
            }
            IsLift = _liftList.Count > 0;
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
                Destroy(target);
            }else if (target.TryGetComponent<Rigidbody>(out var rigidbody)) { 
                rigidbody.isKinematic = false;
                rigidbody.AddForce(-transform.forward * 10, ForceMode.Impulse);
                target.transform.GetComponent<Collider>().isTrigger = false;
            }
            _liftList.Dequeue();
            RefreshLiftQueue();
        }
        public GameObject GetLiftObject() { 
            if (_liftList.Count == 0) return null;
            return _liftList.Peek();
        }
        private void RefreshLiftQueue() { 
            _currentHeight = 0;
            foreach (var target in _liftList)
            {
                target.transform.localPosition = new Vector3(0, _currentHeight, 0);
                _currentHeight += listOffset;
            }
        }
    }
}


