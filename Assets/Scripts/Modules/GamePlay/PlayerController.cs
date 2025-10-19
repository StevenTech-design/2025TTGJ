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

        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private List<GameObject> _liftList = new();
        [SerializeField]
        private float listOffset = 0.2f;
        private float _currentHeight = 0;


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

            _liftList.Add(target);

            target.transform.SetParent(liftArea);
            float height = targetHeight / 2 + _currentHeight;
            target.transform.localRotation = Quaternion.identity;
            target.transform.localPosition = new Vector3(0, height, 0);
            _currentHeight += targetHeight + listOffset;
        }

        public void ToDrop()
        {
            if (_liftList.Count <= 0) return;
            Vector3 dropDirection = (transform.forward + Vector3.up).normalized;
            while (_liftList.Count > 0)
            {
                GameObject target = _liftList[0];
                _liftList.RemoveAt(0);
                target.transform.SetParent(null);
                if (target.TryGetComponent<Liftable>(out var liftable))
                {
                    liftable.OnDrop(dropDirection, dropForce);
                }
            }
            _currentHeight = 0;
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
            if (targetPlant == null) return;
            GameObject seed = GameObject.Instantiate(targetPlant.gameObject);
            seed.GetComponent<Collider>().enabled = true;
            field.ToPlanting(seed.GetComponent<PlantBase>());
        }
        public bool CanEat()
        {
            foreach (var list in _liftList)
            {
                if (list.TryGetComponent<IEatable>(out var eatable) && eatable.CanEat())
                {
                    return true;
                }
            }
            return false;
        }
        public void ToEat()
        {
            PlantBase targetPlant = null;
            foreach (var plant in _liftList)
            {
                if (!plant.TryGetComponent<PlantBase>(out var plantBase)
                    || plantBase.GetCurrentState() != PlantState.Harvest)
                {
                    continue;
                }
                targetPlant = plantBase;
                break;
            }
            if (targetPlant == null) return;
            targetPlant.OnEat();
            ObjectPoolManager.Instance.ReturnGameObjectToPool(targetPlant.gameObject);
            RefleshLiftQueue();
        }
        private void RefleshLiftQueue()
        {
            _currentHeight = 0;

            for (int i = _liftList.Count - 1; i >= 0; i--)
            {
                if (_liftList[i] == null)
                {
                    _liftList.RemoveAt(i);
                    continue;
                }

                float targetHeight = 0;
                if (_liftList[i].TryGetComponent<MeshFilter>(out var filter))
                {
                    targetHeight = filter.sharedMesh.bounds.size.y * _liftList[i].transform.localScale.y;
                }

                float height = targetHeight / 2 + _currentHeight;
                _liftList[i].transform.SetLocalPositionAndRotation(new Vector3(0, height, 0), Quaternion.identity);
                _currentHeight += targetHeight + listOffset;
            }
        }
    }
}


