using TTGJ.Common;

namespace TTGJ.Buff {
    public class SheepTalkBuff : BuffBase {
        public override void StartBuff() {
            base.StartBuff();
            EventManager.Instance.TriggerEvent(EventType.ChangeSheepTalkState, new EventParam<bool> { param = true });
        }
        public override void EndBuff() {
            EventManager.Instance.TriggerEvent(EventType.ChangeSheepTalkState, new EventParam<bool> { param = false });
            base.EndBuff();
        }
    }
}