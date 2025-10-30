using TTGJ.Common;
using TTGJ.Plant;
using UnityEngine;

namespace TTGJ.Interactable
{
    public class Liftable : MonoBehaviour
    {

        protected bool isLiftable = true;
        public ItemType itemType;
        protected Collider collider;
        protected Rigidbody rigidbody;
        private void Awake() {
            Initialize();
        }

        public virtual bool IsLiftable() { return isLiftable; }
        public virtual void OnLift()
        {
            Destroy(GetComponent<Rigidbody>());
            collider.enabled = false;
        }
        public virtual void OnDrop(Vector3 dir, float force)
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rigidbody.useGravity = true;
                rigidbody.isKinematic = false;
                rigidbody.AddForce(dir * force, ForceMode.Impulse);
            }
            collider.enabled = true;
            isLiftable = true;
        }
        protected virtual void Initialize()
        {
            if (collider != null && rigidbody != null) return;
            collider = GetComponent<Collider>();
            rigidbody = GetComponent<Rigidbody>();
        }
        public virtual bool CheckCanLift() { 
            return true;
        }
    }
}