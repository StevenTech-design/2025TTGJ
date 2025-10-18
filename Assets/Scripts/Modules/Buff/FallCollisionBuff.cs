using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TTGJ.Buff
{
    public class FallCollisionBuff : BuffBase
    {
        public Action<Collision> OnCollisionEnterCallback;
        private List<Collision> _collisionList = new List<Collision>();
        private bool isEndBuff = false;
        public override void StartBuff()
        {
            base.StartBuff();
            Debug.Log("FallCollisionBuff StartBuff");
            if (transform.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation |
                               RigidbodyConstraints.FreezePositionX |
                               RigidbodyConstraints.FreezePositionZ;
            }
        }
        public override void EndBuff()
        {
            Debug.Log("FallCollisionBuff EndBuff");
            if (isEndBuff)
            {
                return;
            }
            isEndBuff = true;
            base.EndBuff();
            if (transform.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.constraints = RigidbodyConstraints.None;
                rigidbody.isKinematic = false;
            }
        }

        public async void OnCollisionEnter(Collision collision)
        {
            if (_collisionList.Contains(collision))
            {
                return;
            }
            Debug.Log("FallCollisionBuff OnCollisionEnter");
            OnCollisionEnterCallback?.Invoke(collision);
            _collisionList.Add(collision);
            await UniTask.Delay(3000);
            EndBuff();
        }


    }
}