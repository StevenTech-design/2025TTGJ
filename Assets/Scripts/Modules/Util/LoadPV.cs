using System;
using Cysharp.Threading.Tasks;
using TTGJ.GamePlay;
using TTGJ.UI;
using UnityEngine;

namespace TTGJ.Util
{
    public class LoadPV : MonoBehaviour
    {
        private async System.Threading.Tasks.Task Start() { 
            PVPanel pvPanel = UIManager.Instance.Push<PVPanel>();
            GameObject go = PlayerController.GetActivePlayer().gameObject;
            go.SetActive(false);
            double pvTimeLength = pvPanel.GetPVTimeLength();
            Debug.Log("PVTimeLength: " + pvTimeLength);
            await UniTask.Delay(TimeSpan.FromSeconds(pvTimeLength));
            UIManager.Instance.PopUp();
            go.SetActive(true);
        }
    }
}