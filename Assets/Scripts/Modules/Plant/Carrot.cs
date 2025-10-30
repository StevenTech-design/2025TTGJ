using Cysharp.Threading.Tasks;
using DG.Tweening;
using TTGJ.Audio;
using TTGJ.Buff;
using TTGJ.Common;
using TTGJ.GamePlay;
using TTGJ.Interactable;
using UnityEngine;

namespace TTGJ.Plant
{
    public class Carrot : PlantBase
    {

        public void OnFallCollision(Collider other)
        {
            if (other.gameObject.TryGetComponent<PlantBase>(out var plant)
               && plant.itemType == ItemType.Carrot)
            {

            }
        }
        public override void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<BigModelBuff>(PlayerController.GetActivePlayer().transform);
            buffBase.StartBuff();
        }
        public override void OnTeleport(Transform baseTeleportPos)
        {
            base.OnTeleport(baseTeleportPos);
            rigidbody.centerOfMass = new Vector3(0, -1, 0);
        }
        public override void OnDrop(Vector3 dir, float force)
        {
            base.OnDrop(dir, force);
            rigidbody.centerOfMass = new Vector3(0, -1, 0);
        }
        private void OnCollisionEnter(Collision collision)
        {
            AudioManager.Instance.PlaySFX(Audio.AudioType.Bowling_Pin_Falling_Sound);
        }
    }
}