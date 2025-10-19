using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Mushroom : PlantBase
    {
        private float bounceForce = 10;
        private void OnTriggerStay(Collider other)
        {
            if(currentState < PlantState.Mature) { 
                return;
            }
            if (other.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                return;
            }
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0;
            if (other.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            { 
                rigidbody.AddForce(direction.normalized * bounceForce, ForceMode.Impulse);
            }
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            BuffManager.Instance.RemoveAllBuff(transform);
            FallCollisionBuff fallCollisionBuff = transform.TryAddComponent<FallCollisionBuff>();
            fallCollisionBuff.OnCollisionEnterCallback += OnFallCollision;
            BuffManager.Instance.AddBuff(transform, fallCollisionBuff);
            fallCollisionBuff.StartBuff();
        }
        public void OnFallCollision(Collision collision)
        {
            Debug.Log("Mushroom OnFallCollision play bounce animation" + collision.gameObject.name);
            if(!collision.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                return;
            }
            Vector3 dir = collision.transform.position - transform.position;
            dir.y = 0;
            rigidbody.AddForce(dir.normalized * bounceForce, ForceMode.Impulse);
        }
        public override void OnEat()
        {
            BuffManager.Instance.RemoveAllBuff(transform);
            BuffBase buffBase = PlayerController.Instance.transform.TryAddComponent<ReverseDirBuff>();
            BuffManager.Instance.AddBuff(PlayerController.Instance.transform, buffBase);
        }
    }
}