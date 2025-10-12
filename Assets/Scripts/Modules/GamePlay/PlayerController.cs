using System.Collections.Generic;
using UnityEngine;

namespace TTGJ.GamePlay
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float speed = 3.5f;

        private Rigidbody _rigidbody;

        [SerializeField]
        private Transform liftArea;

        private Queue<GameObject> _liftQueue = new();
        private float _currentHeight = 0;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void ToMove(Vector3 direction) { 
            _rigidbody.MovePosition(_rigidbody.position + speed * Time.fixedDeltaTime * direction);
        }
        public void ToLift(GameObject target) {
            if (target.TryGetComponent<Collider>(out var collider)) { 
                collider.enabled = false;
            }
            _liftQueue.Enqueue(target);
            target.transform.SetParent(liftArea);
            target.transform.localPosition = new Vector3(0, _currentHeight, 0);
            _currentHeight += target.GetComponent<Collider>().bounds.size.y;
        }
    }
}


