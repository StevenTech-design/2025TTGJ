using UnityEngine;

namespace TTGJ.Interactable { 
    [RequireComponent(typeof(IFallCollisionable))]
    public class FallCollisionDetect : MonoBehaviour
    {
        [SerializeField]
        private float fallVelocityThreshold = 10f;
        private void OnCollisionEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<IFallCollisionable>(out var fallCollisionable))
            {
                fallCollisionable.OnFallCollision(other);
            }
        }
    }
}