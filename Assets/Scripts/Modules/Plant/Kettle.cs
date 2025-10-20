using UnityEngine;
using TTGJ.Plant;
using TTGJ.Interactable;
using System;
using TTGJ.Buff;
using TTGJ.Framework;

namespace TTGJ.GamePlay
{
    public class Kettle : Liftable, IEatable
    {
        public bool CanEat()
        {
            return true;
        }

        public void OnEat()
        {
            BuffBase buffBase = BuffManager.Instance.AddBuff<ForwardWateringBuff>(PlayerController.Instance.transform);
            buffBase.OnBuffEndCallback += OnForwardWateringBuffEnd;
            buffBase.StartBuff();
        }
        private async void OnForwardWateringBuffEnd()
        {
            GameObject kettle = await StResources.Instance.LoadAsync<GameObject>("Assets/Res/Prefabs/Kettle.prefab");
            kettle.transform.position = PlayerController.Instance.transform.position - PlayerController.Instance.transform.forward * 0.2f;
            kettle.transform.rotation = PlayerController.Instance.transform.rotation;
            kettle.GetComponent<Rigidbody>().AddForce(-PlayerController.Instance.transform.forward * 3, ForceMode.Impulse);
        }
    }
}