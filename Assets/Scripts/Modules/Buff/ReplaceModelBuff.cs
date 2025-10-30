using TTGJ.Framework;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Buff
{
    public class ReplaceModelBuff : ReplaceModelBuffBase
    {
        public GameObject originalModel;
        private GameObject newModel;
        public override void StartBuff() { 
            base.StartBuff();
            ReplaceModel();
        }
        private void ReplaceModel() { 
            newModel = Instantiate(modelPrefab,PlayerController.GetActivePlayer().modelTransform.parent);
            newModel.SetActive(true);
            newModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            newModel.transform.localScale = Vector3.one;
            originalModel.SetActive(false);
        }
        public override void EndBuff() { 
            originalModel.SetActive(true);
            GameObject.Destroy(newModel);
            base.EndBuff();
        }
    }
}