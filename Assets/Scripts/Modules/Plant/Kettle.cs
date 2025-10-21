using UnityEngine;
using TTGJ.Plant;
using TTGJ.Interactable;
using System;
using TTGJ.Buff;
using TTGJ.Framework;
using TTGJ.GamePlay;
using TTGJ.Generate;

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
        private void OnForwardWateringBuffEnd()
        {
            GameObject kettle = GameObject.Instantiate(StResources.Instance.LoadByResources<GameObject>(ResPathConfig.Plants_Kettle));
            kettle.transform.position = PlayerController.Instance.transform.position - PlayerController.Instance.transform.forward * 2f;
            kettle.transform.rotation = PlayerController.Instance.transform.rotation;
            kettle.GetComponent<Rigidbody>().AddForce(-PlayerController.Instance.transform.forward * 3, ForceMode.Impulse);
        }
    }
}