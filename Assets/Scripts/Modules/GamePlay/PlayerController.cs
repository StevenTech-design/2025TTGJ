using System.Collections.Generic;
using TTGJ.Plant;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.GamePlay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        public float speed = 3.5f;
        [SerializeField]
        private float dropForce = 10;
        [SerializeField]
        private float dropAngle = 60f;

        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private Queue<GameObject> _liftQueue = new();
        [SerializeField]
        private float listOffset = 0.2f;
        private float _currentHeight = 0;


        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
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

            if(target.TryGetComponent<Liftable>(out var liftable))
            {
                liftable.OnLift();
            }
            
            _liftQueue.Enqueue(target);

            target.transform.SetParent(liftArea);
            float height = targetHeight / 2 + _currentHeight;
            target.transform.localRotation = Quaternion.identity;
            target.transform.localPosition = new Vector3(0, height, 0);
            _currentHeight += targetHeight + listOffset;
        }

        public void ToDrop()
        {
            if (_liftQueue.Count <= 0) return;
            Vector3 dropDirection = Quaternion.AngleAxis(dropAngle, transform.right) * transform.forward;
            while (_liftQueue.Count > 0)
            {
                GameObject target = _liftQueue.Dequeue();
                target.transform.SetParent(null);
                if (target.TryGetComponent<Liftable>(out var liftable))
                {
                    liftable.OnDrop(dropDirection.normalized, dropForce);
                }
            }
            _currentHeight = 0;
        }

        public void ToPlant(Field field) { 
            GameObject[] plants = _liftQueue.ToArray();
            PlantBase targetPlant = null;
            foreach (var plant in plants) { 
                if(!plant.TryGetComponent<PlantBase>(out var plantBase)) { 
                    continue;
                }
                targetPlant = plantBase;
                break;
            }
            if(targetPlant == null) return;
            GameObject seed = GameObject.Instantiate(targetPlant.gameObject);
            seed.GetComponent<Collider>().enabled = true;
            field.ToPlanting(seed.GetComponent<PlantBase>());
        }
        public void ToEat() { 
            
        }
    }
}


