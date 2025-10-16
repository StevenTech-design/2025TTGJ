using UnityEngine;
namespace TTGJ.Buff
{
    public class BigHeadBuff : BuffBase
    {
        private float headScale;

        protected override void StartBuff()
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

        protected override void EndBuff()
        {
            DiscardBuff();
            base.EndBuff();
        }
    }
}