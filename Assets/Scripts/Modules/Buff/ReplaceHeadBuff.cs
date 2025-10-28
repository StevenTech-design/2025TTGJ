using TTGJ.Framework;
using TTGJ.GamePlay;
using UnityEngine;

namespace TTGJ.Buff
{
    public class ReplaceHeadBuff : ReplaceModelBuffBase
    {
        private GameObject newModel;
        private Material playerHeadMaterial;
        public override void StartBuff() { 
            base.StartBuff();
            ReplaceHead();
        }
        private void ReplaceHead() { 
            newModel = Instantiate(modelPrefab,PlayerController.Instance.headTransform);
            newModel.SetActive(true);
            newModel.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            newModel.transform.localScale = Vector3.one;
            HideHead();
        }
        private void HideHead() {
            Renderer renderer = PlayerController.Instance.transform.GetComponentInChildren<Renderer>();
            if (renderer == null) {
                return;
            }
            // Loop through the renderer's materials to find the one named "PlayerHead"
            Material[] materials = renderer.materials;
            foreach (var mat in materials)
            {
                if (mat != null && mat.name.Contains("PlayerHead"))
                {
                    playerHeadMaterial = mat;
                    break;
                }
            }
            if (playerHeadMaterial == null) {
                return;
            }
            playerHeadMaterial.SetFloat("_Alpha", 0);
        }

        public override void EndBuff() { 
            GameObject.Destroy(newModel);
            RevealHead();
            base.EndBuff();
        }
        public void RevealHead() { 
            if (playerHeadMaterial == null) {
                return;
            }
            playerHeadMaterial.SetFloat("_Alpha", 1);
        }
    }
}