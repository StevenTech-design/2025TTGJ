using System.Collections;
using System.Collections.Generic;
using Dm.TwistedFate.TexasHoldem;
using TTGJ.Framework;
using UnityEngine;

namespace TTGJ
{ 
    public class GameRoot : MonoBehaviour
    {
        private async void Start()
        { 
            var player = await StResources.Instance.Load<GameObject>(ResPathConfig.Art_Cube);
            Debug.Log(player);
        }
    }
}