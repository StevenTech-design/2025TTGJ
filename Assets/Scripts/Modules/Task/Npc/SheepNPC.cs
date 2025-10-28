using TTGJ.Buff;
using UnityEngine;

namespace TTGJ.Task { 
    public class SheepNPC : NPCBase {
        protected override void OnTriggerEnter(Collider other) {
            if (!other.gameObject.TryGetComponent<SheepTalkBuff>(out var sheepTalkBuff)) { 
                return;
            }
            base.OnTriggerEnter(other);
        }
    }
}