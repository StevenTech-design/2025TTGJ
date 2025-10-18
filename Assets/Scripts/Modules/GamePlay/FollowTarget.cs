using UnityEngine;

namespace TTGJ.GamePlay
{
    public class FollowTarget : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        
        
        public Vector3 offset;
        private void Start()
        {
            offset = transform.position - target.position;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            transform.position = desiredPosition;
        }
    }
}