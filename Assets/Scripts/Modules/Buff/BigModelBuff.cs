using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Buff
{
    public class BigModelBuff : BuffBase
    {
        private float modelScale = 1.5f;
        private Vector3 cameraOffset = new Vector3(0, 1, 0);
        public override void StartBuff()
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
            Camera.main.GetComponent<FollowTarget>().offset += cameraOffset;
        }
        private void DiscardBuff()
        { 
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            player.transform.localScale /= modelScale;
            Camera.main.GetComponent<FollowTarget>().offset -= cameraOffset;
        }
        public override void EndBuff()
        {
            DiscardBuff();
            base.EndBuff();
        }
    }
}