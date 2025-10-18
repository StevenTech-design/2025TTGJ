using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Mushroom : PlantBase, IFallCollisionable
    {
        
        [SerializeField]
        private float bounceForce = 10;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                return;
            }
            Vector3 direction = other.transform.position - transform.position;
            if (other.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            { 
                rigidbody.AddForce(direction.normalized * bounceForce, ForceMode.Impulse);
            }
        }
        public void OnFallCollision(Collider other)
        {
            Debug.Log("Mushroom OnFallCollision play bounce animation");
            if(!other.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                return;
            }
            Vector3 dir = other.transform.position - transform.position;
            rigidbody.AddForce(dir.normalized * bounceForce, ForceMode.Impulse);
        }
    }
}