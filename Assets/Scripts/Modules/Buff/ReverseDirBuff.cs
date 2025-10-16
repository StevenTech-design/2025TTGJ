using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Buff
{
    public class ReverseDirBuff : BuffBase
    {
        private MoveKeyCode reverseKeyCode = new MoveKeyCode()
        {
            forward = KeyCode.S,
            back = KeyCode.W,
            left = KeyCode.D,
            right = KeyCode.A
        };
        private MoveKeyCode originalKeyCode;
        protected override void StartBuff()
        {
            base.StartBuff();
            originalKeyCode = InputSystem.Instance.GetMoveKeyCode();
            InputSystem.Instance.SetMoveKeyCode(reverseKeyCode);
        }
        protected override void EndBuff()
        {
            InputSystem.Instance.SetMoveKeyCode(originalKeyCode);
            base.EndBuff();
        }
    }
}