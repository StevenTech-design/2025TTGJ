using TTGJ.GamePlay;

namespace TTGJ.Buff
{
    public class FastMoveBuff : BuffBase
    {
        private float fastMoveRatio = 1.5f;

        protected override void StartBuff()
        {
            base.StartBuff();
            FastMove();
        }

        private void FastMove()
        {
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            player.speed *= fastMoveRatio;
        }

        protected override void EndBuff()
        {
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            player.speed /= fastMoveRatio;
            base.EndBuff();
        }
        
    }
}