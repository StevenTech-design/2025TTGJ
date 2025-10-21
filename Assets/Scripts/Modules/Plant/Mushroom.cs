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
        private float bounceForce = 5;
        private void OnCollisionEnter(Collision collision)
        {
            BounceObject(collision.collider);
        }
        private void OnTriggerStay(Collider other)
        {
            BounceObject(other);
        }
        private void BounceObject(Collider collider)
        {
            if (currentState < PlantState.Mature)
            {
                return;
            }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                return;
            }
            Vector3 direction = collider.transform.position - transform.position;
            if (collider.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.AddForce(bounceForce * Time.deltaTime * direction.normalized, ForceMode.Impulse);
            }
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            FallCollisionBuff fallCollisionBuff = BuffManager.Instance.AddBuff<FallCollisionBuff>(transform);
            fallCollisionBuff.OnCollisionEnterCallback += OnFallCollision;
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
            rigidbody.AddForce(dir.normalized * bounceForce * Time.deltaTime, ForceMode.Impulse);
        }
        public override void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<ReverseDirBuff>(PlayerController.Instance.transform);
            buffBase.StartBuff();
        }
    }
}