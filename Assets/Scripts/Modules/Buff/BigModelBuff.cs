using TTGJ.GamePlay;

namespace TTGJ.Buff
{
    public class BigModelBuff : BuffBase
    {
        private float modelScale = 1.5f;
        protected override void StartBuff()
        {
            base.StartBuff();
            ToBigModel();
        }
        private void ToBigModel()
        {
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            player.transform.localScale *= modelScale;
        }
        private void DiscardBuff()
        { 
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            player.transform.localScale /= modelScale;
        }
        protected override void EndBuff()
        {
            DiscardBuff();
            base.EndBuff();
        }
    }
}