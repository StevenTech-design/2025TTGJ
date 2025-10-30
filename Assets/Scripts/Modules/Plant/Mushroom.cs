using TTGJ.Audio;
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
        private void OnTriggerEnter(Collider other)
        {
            BounceObject(other);
        }
        private void BounceObject(Collider collider)
        {
            Debug.Log("Mushroom BounceObject: " + collider.gameObject.name);
            if (currentState < PlantState.Mature)
            {
                return;
            }
            if (collider.gameObject.layer == LayerMask.NameToLayer("Building"))
            {
                return;
            }
            Vector3 direction = collider.transform.position - transform.position;
            direction.y = Mathf.Max(direction.y, 0);
            Debug.Log("Mushroom BounceObject direction: " + direction);
            AudioManager.Instance.PlaySFX(Audio.AudioType.Q_Bouncy_Crop);
            if (collider.gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.AddForce(bounceForce * direction.normalized, ForceMode.Impulse);
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
            rigidbody.AddForce(dir.normalized * bounceForce, ForceMode.Impulse);
        }
        public override void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<ReverseDirBuff>(PlayerController.GetActivePlayer().transform);
            buffBase.StartBuff();
            FastMoveBuff buff = BuffManager.Instance.AddBuff<FastMoveBuff>(PlayerController.GetActivePlayer().transform,false);
            buff.StartBuff();
        }
    }
}