using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Mushroom : PlantBase
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
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            FallCollisionBuff fallCollisionBuff = new FallCollisionBuff();
            fallCollisionBuff.OnCollisionEnterCallback += OnFallCollision;
            BuffManager.Instance.AddBuff(transform, fallCollisionBuff);
        }
        public void OnFallCollision(Collision collision)
        {
            Debug.Log("Mushroom OnFallCollision play bounce animation");
            if(!collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                return;
            }
            Vector3 dir = collision.transform.position - transform.position;
            rigidbody.AddForce(dir.normalized * bounceForce, ForceMode.Impulse);
        }
        public override void OnEat()
        {
            BuffBase buffBase = PlayerController.Instance.transform.TryAddComponent<ReverseDirBuff>();
            BuffManager.Instance.AddBuff(PlayerController.Instance.transform, buffBase);
        }
    }
}