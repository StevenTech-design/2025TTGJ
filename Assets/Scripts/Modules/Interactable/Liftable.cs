using UnityEngine;

namespace TTGJ.Interactable
{
    public class Liftable : MonoBehaviour
    {

        protected bool isLiftable = true;
        protected Collider collider;
        protected Rigidbody rigidbody;
        private void Awake() {
            Initialize();
        }

        public virtual bool IsLiftable() { return isLiftable; }
        public virtual void OnLift()
        {

            collider.enabled = false;
            rigidbody.isKinematic = true;
            isLiftable = false;
        }
        public virtual void OnDrop(Vector3 dir, float force)
        {
            collider.enabled = true;
            rigidbody.isKinematic = false;
            rigidbody.AddForce(dir * force);
            isLiftable = true;
        }
        protected virtual void Initialize()
        {
            if (collider != null && rigidbody != null) return;
            collider = GetComponentInChildren<Collider>();
            rigidbody = GetComponentInChildren<Rigidbody>();
        }
    }
}