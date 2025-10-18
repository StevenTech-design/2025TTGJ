using TTGJ.GamePlay;

namespace TTGJ.Buff
{
    public class FastMoveBuff : BuffBase
    {
        private float fastMoveRatio = 1.5f;

        public override void StartBuff()
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

        public override void EndBuff()
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