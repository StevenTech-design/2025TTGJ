using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Buff
{
    public class SmallModelBuff : BuffBase
    {
        private float modelScale = 0.5f;
        private Vector3 cameraOffset = new Vector3(0, -0.5f, 0);
        public override void StartBuff()
        {
            base.StartBuff();
            ToSmallModel();
        }
        private void ToSmallModel()
        {
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            player.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
            Camera.main.GetComponent<FollowTarget>().offset += cameraOffset;
        }
        private void DiscardBuff()
        {
            Debug.Log("DiscardBuff");
            PlayerController player = GetBuffTarget();
            if (player == null)
            {
                return;
            }
            Debug.Log("FollowTarget");
            player.transform.localScale = Vector3.one;
            Camera.main.GetComponent<FollowTarget>().offset -= cameraOffset;
        }
        public override void EndBuff()
        {
            DiscardBuff();
            base.EndBuff();
        }
    }
}