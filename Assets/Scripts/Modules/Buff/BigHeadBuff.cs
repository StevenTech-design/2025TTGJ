using UnityEngine;
namespace TTGJ.Buff
{
    public class BigHeadBuff : BuffBase
    {
        private float headScale;

        public override void StartBuff()
        {
            base.StartBuff();
            ToBigHead();
        }
        private void ToBigHead()
        {
            //TODO:scale player head;
        }
        private void DiscardBuff()
        { 
            
        }

        public override void EndBuff()
        {
            DiscardBuff();
            base.EndBuff();
        }
    }
}