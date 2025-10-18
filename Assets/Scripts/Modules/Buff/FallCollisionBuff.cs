using System;
using UnityEngine;

namespace TTGJ.Buff { 
    public class FallCollisionBuff : BuffBase { 
        public Action<Collision> OnCollisionEnterCallback;
        public override void StartBuff() { 
            base.StartBuff();
            if (transform.TryGetComponent<Rigidbody>(out var rigidbody)) { 
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
                rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
            }
        }
        public override void EndBuff()
        {
            base.EndBuff();
            if (transform.TryGetComponent<Rigidbody>(out var rigidbody)) { 
                rigidbody.constraints = RigidbodyConstraints.None;
            }
        }

        public void OnCollisionEnter(Collision collision) { 
            OnCollisionEnterCallback?.Invoke(collision);
            EndBuff();
        }


    }
}