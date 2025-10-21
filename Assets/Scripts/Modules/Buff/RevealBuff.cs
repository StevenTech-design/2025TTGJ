using TTGJ.Common;

namespace TTGJ.Buff
{
    public class RevealBuff : BuffBase
    {
        public override void StartBuff()
        {
            base.StartBuff();
            Reveal();
        }
        private void Reveal()
        {
            LayerUtility.AddLayersToMainCamera("HideInWorld");
        }
        public override void EndBuff()
        {
            base.EndBuff();
            LayerUtility.RemoveLayersFromMainCamera("HideInWorld");
        }
    }
}